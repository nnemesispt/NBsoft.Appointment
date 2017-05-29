using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.Common
{
    static class Importer
    {
        public static event EventHandler<Common.LogEventArgs> EventLog;

        public static void ImportClientData(string excelFileName)
        {
            //string excelConnString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0\"", excelFileName);
            string excelConnString = string.Format("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {0};Extended Properties = \"Excel 8.0;HDR=Yes;IMEX=1\";", excelFileName);

            string datePattern = @"(0?[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d";
            string yearMonthPattern = @"(0[1-9]|1[012])[- /.](19|20)\d\d";
            string postalcodePattern = @"^\d{4}-\d{3}";


            DAL.DataModel.AppointmentType appointmentType = Globals.Db.GetAppointmentType().FirstOrDefault();
            if (appointmentType == null)
                throw new ApplicationException("Não existem Tipos de Consulta configurados");
            DAL.DataModel.Doctor doctor = Globals.Db.GetDoctor().FirstOrDefault();
            if (doctor == null)
                throw new ApplicationException("Não existem Doutores configurados");

            List<DAL.DataModel.Customer> customersToCreate = new List<DAL.DataModel.Customer>();
            using (OleDbConnection excelConnection = new OleDbConnection(excelConnString))
            {
                using (OleDbCommand cmd = new OleDbCommand("Select [Ref],[Nome],[Data nascimento], [Contacto], [Morada], [Categorias], [Obs] from [Folha1$]", excelConnection))
                {
                    excelConnection.Open();
                    int lineCount = 0;
                    using (OleDbDataReader dReader = cmd.ExecuteReader())
                    {
                        while (dReader.Read())
                        {
                            string myRef = dReader.GetValue(0).ToString();
                            string myName = dReader.GetValue(1).ToString();
                            string myBDay = dReader.GetValue(2).ToString();
                            string myContact = dReader.GetValue(3).ToString();
                            string myAddress = dReader.GetValue(4).ToString();
                            string myCategory = dReader.GetValue(5).ToString();
                            string myComments = dReader.GetValue(6).ToString();

                            if (myRef == "" || myName == "")
                                continue;
                            lineCount++;

                            // Birth Date
                            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("pt-PT");
                            DateTime bday;
                            if (myBDay != "")
                                bday = DateTime.Parse(myBDay, ci);
                            else
                                bday = new DateTime(1900, 01, 01);

                            // Contact
                            string[] contacts = myContact.Split('\n');
                            string mobile = "";
                            string phone = "";

                            foreach (var item in contacts)
                            {
                                if (item.StartsWith("9"))
                                    mobile = item;
                                else
                                {
                                    if (phone == "")
                                        phone += item;
                                    else
                                        phone += " " + item;
                                }

                            }

                            // Address
                            string[] addresses = myAddress.Split('\n');
                            string address = "";
                            string postalcode = "";
                            string city = "";
                            
                            foreach (var item in addresses)
                            {
                                Match m = Regex.Match(item, postalcodePattern);
                                if (m.Success)
                                {
                                    // Postal Code
                                    postalcode = item.Substring(m.Index, 8);
                                    string match = m.Value;
                                    city = item.Replace(m.Value, "").Trim();
                                }
                                else
                                {
                                    if (address == "")
                                        address = item;
                                    else
                                        address += " " + item;

                                }

                            }

                            Log(string.Format("Creating Customer: {0}", myName));
                            if (myName == "")
                                Console.WriteLine("Empty Name");


                            // Next appointment                            
                            DateTime? nextappointment;
                            if (myComments != "")
                            {
                                var datematch = Regex.Matches(myComments, datePattern);
                                if (datematch.Count > 0)
                                {
                                    string datestring = datematch[datematch.Count - 1].Value;
                                    DateTime dt;
                                    if (DateTime.TryParse(datestring, out dt))
                                        nextappointment = dt;
                                    else
                                        nextappointment = null;
                                }
                                else
                                {
                                    var yearMonthMatch = Regex.Matches(myComments, yearMonthPattern);
                                    if (yearMonthMatch.Count > 0)
                                    {
                                        string yearMonth = yearMonthMatch[yearMonthMatch.Count - 1].Value;
                                        DateTime dt;
                                        if (DateTime.TryParse("01-" + yearMonth, out dt))
                                            nextappointment = dt;
                                        else
                                            nextappointment = null;
                                    }
                                    else
                                        nextappointment = null;
                                }
                            }
                            else
                                nextappointment = null;

                            // create Customer
                            DAL.DataModel.Customer c = new DAL.DataModel.Customer()
                            {
                                CreationDate = DateTime.Now,
                                Name = myName.Trim(),
                                BirthDate = bday,
                                MobilePhone = mobile.Trim(),
                                Telephone = phone.Trim(),
                                Address = address.Trim(),
                                PostalCode = postalcode.Trim(),
                                City = city.Trim(),
                                DrivingLicenseType = myCategory,
                                Comments = myComments,
                                TaxIdNumber = (999000000 + lineCount).ToString(),
                                NextAppointment = nextappointment
                            };
                            //long[] cid = Globals.Db.SetCustomer(new DAL.DataModel.Customer[] { c });
                            //c.Id = cid[0];
                            customersToCreate.Add(c);

                            // Appointments
                            string[] appointments = myRef.Split('\n');

                            List<AppointmentInfo> apps = new List<Common.Importer.AppointmentInfo>();
                            foreach (var item in appointments)
                            {
                                if (item == "")
                                    continue;

                                


                                AppointmentInfo ai = new AppointmentInfo();
                                string[] appointmentInfo = item.Split('/');
                                string rnumber = Regex.Replace(appointmentInfo[0].Trim(), @"[^\d]", "");
                                //bool isNumber = appointmentInfo[0].Trim().All(char.IsDigit);

                                long number = 0;
                                if (!long.TryParse(rnumber, out number))
                                    continue;
                                ai.Number = number.ToString();
                                if (appointmentInfo.Length > 1)
                                    ai.Year = appointmentInfo[1].Trim();
                                apps.Add(ai);
                            }                            
                            var matches = Regex.Matches(myCategory, datePattern);
                            int lastpos = 0;
                            string desc = "";
                            string date = "";
                            if (matches.Count > 0)
                            {
                                for (int i = 0; i < matches.Count; i++)
                                {
                                    date = matches[i].Value;
                                    desc = myCategory.Substring(lastpos, matches[i].Index - lastpos);
                                    lastpos = matches[i].Index + matches[i].Value.Length;

                                    DateTime dt = DateTime.Parse(date, ci);
                                    string y = dt.Year.ToString().Substring(2);
                                    date = dt.ToString("yyyy-MM-dd");
                                                                        
                                    var a = apps.Where(m => m.Year == y).FirstOrDefault();
                                    if (a != null)
                                    {
                                        a.Date = dt.ToString("yyyy-MM-dd");
                                        a.Description = desc;
                                    }

                                    //foreach (var item in apps)
                                    //{
                                    //    if (item.Date == null)
                                    //    {
                                    //        item.Date = date;
                                    //        item.Description = myCategory;
                                    //    }

                                    //}


                                }
                            }
                            else
                                desc = myCategory;

                            
                            List<DAL.DataModel.Appointment> appointmentsToCreate = new List<DAL.DataModel.Appointment>();
                            
                            foreach (var app in apps)
                            {
                                if (app.Number == "975")
                                    Console.WriteLine("ss");
                                if (app.Date == null || app.Date =="")
                                {
                                    int pyear=0;
                                    if (int.TryParse(app.Year, out pyear))
                                    {
                                        if (pyear < 100)
                                            pyear = pyear + 2000;
                                        app.Date = new DateTime(pyear, 1, 1).ToString("yyyy-MM-dd");
                                    }
                                    else
                                    {
                                        if (date != "")
                                            app.Date = date;
                                        else
                                            app.Date = "2001-01-01";
                                    }                                   
                                    app.Description = myCategory;
                                }

                                DateTime adate = DateTime.ParseExact(app.Date, "yyyy-MM-dd", ci);
                                string rnumber = Regex.Replace(app.Number, @"[^\d]", "");
                                long anumber = long.Parse(rnumber);

                                DAL.DataModel.Appointment a = new DAL.DataModel.Appointment()
                                {
                                    Id_AppointmentType = appointmentType.Id,
                                    Id_Customer = c.Id,
                                    Id_Doctor = doctor.Id,
                                    Id_User = Globals.LoggedUser.Id,

                                    Description = app.Description,
                                    AppointmentDate = adate,
                                    Number = anumber,

                                    ClientDiscount = 0,
                                    Exchange = 1,
                                    FinanceDiscount = 0,                                    
                                    PaymentType = "NUM",
                                    Report = "",
                                    TotalProducts = 0,
                                    CreationDate = DateTime.Now,
                                    Coin = "EUR",
                                    ComercialDiscountVal = 0,
                                    Comments = myComments,
                                    VATValue = 0
                                };
                                appointmentsToCreate.Add(a);
                            }
                            c.Appointment = appointmentsToCreate;
                            //long[] res = Globals.Db.SetAppointment(appointmentsToCreate.ToArray());
                            //if (res.Length != appointmentsToCreate.Count)
                                //throw new ApplicationException("DB Insert Failed");

                            //Console.WriteLine("ss");
                        }
                    }

                }
            }

            Log("Saving Customers to database...");            
            long[] CustomerIds = Globals.Db.SetCustomer(customersToCreate.ToArray());
            Log("... Done!");
            Console.WriteLine(customersToCreate.Count);
        }

        class AppointmentInfo
        {
            public string Number { get; set; }
            public string Year { get; set; }
            public string Date { get; set; }
            public string Description { get; set; }
        }

        private static void Log(string text)
        {
            OnEventLog(new Common.LogEventArgs("Importer", text));
        }

        private static void OnEventLog(LogEventArgs e)
        {
            EventLog?.Invoke("Importer", e);
        }
    }
}
