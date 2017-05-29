using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NBsoft.Appointment.WPF.Content
{
    /// <summary>
    /// Interaction logic for SettingsDebug.xaml
    /// </summary>
    public partial class SettingsDebug : UserControl
    {
        private Random gen = new Random();

        public SettingsDebug()
        {
            InitializeComponent();
            dtp.SelectedDate = DateTime.Now;
        }
                
        private void BtnCreateCustomerData_Click(object sender, RoutedEventArgs e)
        {
            //Clientes
            List<DAL.DataModel.Customer> customers = new List<DAL.DataModel.Customer>();
            for (int i = 0; i < 300; i++)
            {
                DAL.DataModel.Customer c = new DAL.DataModel.Customer()
                {
                    Address = string.Format("Customer {0} Address", i),
                    BirthDate = DateTime.Today.AddYears(-20),
                    City = string.Format("{0} City", i),
                    Comments = string.Format("Comments for customer {0}", i),
                    Contact = string.Format("Contact {0}", i),
                    Country = string.Format("Country {0}", i),
                    CreationDate = DateTime.Now,
                    DrivingLicense = string.Format("{0,5}", i),
                    DrivingLicenseDate = new DateTime(2001, 02, 24),
                    DrivingLicenseType = "A,B",
                    EMail = string.Format("customeremail{0}@mailserver.com", i),
                    Fax = string.Format("{0,9}", i),
                    IBAN = string.Format("{0,9}", i),
                    MobilePhone = string.Format("{0,9}", i),
                    Name = string.Format("Customer {0, 5}'s Name ", i),
                    PostalCode = "1234-123",
                    TaxIdNumber = string.Format("{0,9}", i),
                    Telephone = string.Format("{0,9}", i),
                    URL = string.Format("http://wwww.customer{0}url.com", i),
                    NextAppointment = DateTime.Today.AddDays(20)
                };
                customers.Add(c);
            }

            Globals.Db.SetCustomer(customers.ToArray());

        }

        private void BtnCreateDoctorData_Click(object sender, RoutedEventArgs e)
        {
            //Doutores
            List<DAL.DataModel.Doctor> doctors = new List<DAL.DataModel.Doctor>();
            for (int i = 0; i < 300; i++)
            {
                DAL.DataModel.Doctor c = new DAL.DataModel.Doctor()
                {
                    Address = string.Format("Doctor {0} Address", i),
                    City = string.Format("{0} City", i),
                    Comments = string.Format("Comments for Doctor {0}", i),
                    Contact = string.Format("Contact {0}", i),
                    Country = string.Format("Country {0}", i),
                    CreationDate = DateTime.Now,                    
                    EMail = string.Format("doctoremail{0}@mailserver.com", i),
                    Fax = string.Format("{0,9}", i),
                    IBAN = string.Format("{0,9}", i),
                    MobilePhone = string.Format("{0,9}", i),
                    Name = string.Format("Doctor {0, 5}'s Name ", i),
                    PostalCode = "1234-123",
                    TaxIdNumber = string.Format("{0,9}", i),
                    Telephone = string.Format("{0,9}", i),
                    URL = string.Format("http://wwww.doctor{0}url.com", i)                    
                };
                doctors.Add(c);
            }

            Globals.Db.SetDoctor(doctors.ToArray());
        }

        private void BtnCreateAppointmentType_Click(object sender, RoutedEventArgs e)
        {
            //AppointmentType
            List<DAL.DataModel.AppointmentType> atypes = new List<DAL.DataModel.AppointmentType>();
            for (int i = 0; i < 50; i++)
            {
                DAL.DataModel.AppointmentType c = new DAL.DataModel.AppointmentType()
                {
                    CreationDate = DateTime.Now,                 
                    Name = string.Format("Appointment Type {0, 5}'s Name ", i)                    
                    
                };
                atypes.Add(c);
            }

            Globals.Db.SetAppointmentType(atypes.ToArray());
        }

        private void BtnCreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            int year = dtp.DisplayDate.Year;

            long maxcostumerId = Globals.Db.GetCustomer().Max(m => m.Id);
            long msxdoctorId = Globals.Db.GetDoctor().Max(m => m.Id);
            long msxuserId = Globals.Db.GetUser().Max(m => m.Id);
            long maxappointmentTypeId = Globals.Db.GetAppointmentType().Max(m => m.Id);



            //Consultas 
            List<DAL.DataModel.Appointment> appointments = new List<DAL.DataModel.Appointment>();
            for (int i = 0; i < 100; i++)
            {
                DAL.DataModel.Appointment c = new DAL.DataModel.Appointment()
                {
                    CreationDate = DateTime.Now,
                    AppointmentDate = RandomDay(year),
                    ClientDiscount = 0,
                    Coin = "Eur",
                    ComercialDiscountVal = 0,
                    Comments = string.Format("Coments for appointment number {0,6}", i),
                    Description = "my appointment number",
                    Id_AppointmentType = gen.Next(1, (int)maxappointmentTypeId),
                    Id_Customer = gen.Next(1, (int)maxcostumerId),
                    Id_Doctor = gen.Next(1, (int)msxdoctorId),
                    Id_User = gen.Next(1, (int)msxuserId),
                    Exchange = 1,
                    FinanceDiscount = 0,
                    Number = i + 1,
                    Report = "",
                    PaymentType = "PP",
                    TotalProducts = gen.Next(100, 1000) / 100,
                    VATValue = 23
                };
                appointments.Add(c);
            }

            Globals.Db.SetAppointment(appointments.ToArray());
        }

        
        DateTime RandomDay(int year)
        {
            DateTime start = new DateTime(year, 1, 1);
            DateTime end = start.AddYears(1).AddDays(-1);
            int range = (end - start).Days;
            return start.AddDays(gen.Next(range));
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(TxtExcelFile.Text);
            if (!fi.Exists)
            {
                ModernDialog.ShowMessage("Ficheiro não existe", Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }

            ImportAsync(fi.FullName);
            

        }
        private void ShowWait()
        {
            WaitGrid.Visibility = Visibility.Visible;
            WaitAnimation.IsActive = true;
        }
        private void HideWait()
        {
            WaitAnimation.IsActive = false;
            WaitGrid.Visibility = Visibility.Collapsed;
        }

        private void ImportAsync(string filename)
        {
            ShowWait();

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.RunWorkerAsync(filename);
            //try
            //{
            
            //}
            //catch(Exception ex01)
            //{
            //    Globals.LogError(ex01);

            //    string msg = string.Format("Erro: {0}\n\r{1}", ex01.Source, ex01.Message);
            //    ModernDialog.ShowMessage(msg, Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
            //}
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string filename = e.Argument.ToString();
            Common.Importer.EventLog += Importer_EventLog;
            Common.Importer.ImportClientData(filename);            
        }
        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            if (e.Error != null)
            {
                string msg = string.Format("Erro: {0}\n\r{1}", e.Error.Source, e.Error.Message);
                ModernDialog.ShowMessage(msg, Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
            }
        }
        private void Importer_EventLog(object sender, Common.LogEventArgs e)
        {
            string text = string.Format("{0} > {1}", e.LogDate.ToString("HH:mm:ss.fff"), e.LogText);
            Globals.Log(e.Sender, e.LogType, e.LogText);
            this.Dispatcher.Invoke(new Action(() => LblStatus.Content = text));


        }
    }
}
