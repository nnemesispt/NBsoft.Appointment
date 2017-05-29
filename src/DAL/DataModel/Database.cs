using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.DAL.DataModel
{
    public class Database
    {
        private AppointmentEntities db;

        public Database(string sqliteFile)
        {
            string SqliteConnString = string.Format("Data Source = {0}; Version = 3; UseUTF16Encoding = True;", sqliteFile);
            string metadata = string.Format("metadata=res://*/DataModel.AppointmentModel.csdl|res://*/DataModel.AppointmentModel.ssdl|res://*/DataModel.AppointmentModel.msl;provider=System.Data.SQLite.EF6; provider connection string=\"{0}\"", SqliteConnString);

            db = new AppointmentEntities(metadata);
        }

        public MainOptions GetOptions()
        {
            db.OpenDb();
            try
            {
                var opt = db.MainOptions.FirstOrDefault();
                if (opt == null)
                {
                    opt = new DataModel.MainOptions()
                    {
                        CompanyName = "nbsoft",
                        Permissions = ""
                    };
                    db.MainOptions.Add(opt);
                    db.SaveChanges();
                    opt = db.MainOptions.FirstOrDefault();
                }
                return opt;
            }
            finally { db.CloseDb(); }
        }

    
        public void SetOptions(MainOptions options)
        {
            db.OpenDb();
            try
            {
                options.Id = 1;
                var dbo = db.MainOptions.Find(options.Id);
                if (dbo != null)
                    //db.Customer.Attach(item);
                    //db.Entry(item).State = EntityState.Modified;
                    db.Entry(dbo).CurrentValues.SetValues(options);

                db.SaveChanges();                
            }
            finally { db.CloseDb(); }
        }      

        public User[] GetUser()
        {
            db.OpenDb();
            try
            {
                return db.User.ToArray();
            }
            finally { db.CloseDb(); }
        }
        public long[] SetUser(User[] users)
        {
            db.OpenDb();
            try
            {
                User[] newObjetcs = users.Where(m => m.Id == 0).ToArray();
                User[] editObjects = users.Where(m => m.Id > 0).ToArray();
                if (newObjetcs.Length > 0)
                    db.User.AddRange(newObjetcs);
                if (editObjects.Length > 0)
                {
                    foreach (var item in editObjects)
                    {
                        var dbo = db.User.Find(item.Id);
                        if (dbo != null)
                            //db.Customer.Attach(item);
                            //db.Entry(item).State = EntityState.Modified;
                            db.Entry(dbo).CurrentValues.SetValues(item);
                    }
                }
                db.SaveChanges();
                long[] retval = users.Select(m => m.Id).ToArray();
                return retval;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException exception)
            {
                string verror = "Validation Error:";
                foreach (var item in exception.EntityValidationErrors)
                {
                    verror += "\n - " + item.ValidationErrors.First().ErrorMessage;
                }

                throw new Exception(verror);
            }
            finally { db.CloseDb(); }
        }
        public void DeleteUser(long[] userIds)
        {
            if (userIds != null && userIds.Length > 0)
            {
                foreach (var item in userIds)
                {
                    User u = db.User.Where(m => m.Id == item).FirstOrDefault();
                    if (u == null)
                        throw new ArgumentException("Invalid User Id");

                    db.User.Remove(u);
                }
            }
            db.SaveChanges();
        }

        public Customer[] GetCustomer()
        {
            db.OpenDb();
            try
            {
                return db.Customer.ToArray();
            }
            finally { db.CloseDb(); }
        }
        public long[] SetCustomer(Customer[] customers)
        {
            db.OpenDb();
            try
            {
                Customer[] newObjetcs = customers.Where(m => m.Id == 0).ToArray();
                Customer[] editObjects = customers.Where(m => m.Id > 0).ToArray();
                if (newObjetcs.Length > 0)
                    db.Customer.AddRange(newObjetcs);
                if (editObjects.Length > 0)
                {
                    foreach (var item in editObjects)
                    {
                        var dbo = db.Customer.Find(item.Id);
                        if (dbo != null)
                            //db.Customer.Attach(item);
                            //db.Entry(item).State = EntityState.Modified;
                            db.Entry(dbo).CurrentValues.SetValues(item);
                    }
                }
                db.SaveChanges();
                long[] retval = customers.Select(m => m.Id).ToArray();
                return retval;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException exception)
            {
                string verror = "Validation Error:";
                foreach (var item in exception.EntityValidationErrors)
                {
                    verror += "\n - " + item.ValidationErrors.First().ErrorMessage;
                }

                throw new Exception(verror);
            }
            finally { db.CloseDb(); }
        }                
        public void DeleteCustomer(long[] customerIds)
        {
            if (customerIds != null && customerIds.Length > 0)
            {
                foreach (var item in customerIds)
                {
                    Customer c = db.Customer.Where(m => m.Id == item).FirstOrDefault();
                    if (c == null)
                        throw new ArgumentException("Invalid Customer Id");

                    db.Customer.Remove(c);
                }
            }
            db.SaveChanges();
        }

        public Doctor[] GetDoctor()
        {
            db.OpenDb();
            try
            {
                return db.Doctor.ToArray();
            }
            finally { db.CloseDb(); }
        }
        public long[] SetDoctor(Doctor[] doctors)
        {
            db.OpenDb();
            try
            {
                Doctor[] newObjetcs = doctors.Where(m => m.Id == 0).ToArray();
                Doctor[] editObjects = doctors.Where(m => m.Id > 0).ToArray();
                if (newObjetcs.Length > 0)
                    db.Doctor.AddRange(newObjetcs);
                if (editObjects.Length > 0)
                {
                    foreach (var item in editObjects)
                    {
                        var dbo = db.Doctor.Find(item.Id);
                        if (dbo != null)
                            //db.Customer.Attach(item);
                            //db.Entry(item).State = EntityState.Modified;
                            db.Entry(dbo).CurrentValues.SetValues(item);
                    }
                }
                db.SaveChanges();
                long[] retval = doctors.Select(m => m.Id).ToArray();
                return retval;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException exception)
            {
                string verror = "Validation Error:";
                foreach (var item in exception.EntityValidationErrors)
                {
                    verror += "\n - " + item.ValidationErrors.First().ErrorMessage;
                }

                throw new Exception(verror);
            }
            finally { db.CloseDb(); }
        }
        public void DeleteDoctor(long[] doctorIds)
        {
            if (doctorIds != null && doctorIds.Length > 0)
            {
                foreach (var item in doctorIds)
                {
                    Doctor c = db.Doctor.Where(m => m.Id == item).FirstOrDefault();
                    if (c == null)
                        throw new ArgumentException("Invalid Doctor Id");

                    db.Doctor.Remove(c);
                }
            }
            db.SaveChanges();
        }
        
        public AppointmentType[] GetAppointmentType()
        {
            db.OpenDb();
            try
            {
                return db.AppointmentType.ToArray();
            }
            finally { db.CloseDb(); }
        }
        public long[] SetAppointmentType(AppointmentType[] appointmentTypes)
        {
            db.OpenDb();
            try
            {
                AppointmentType[] newObjetcs = appointmentTypes.Where(m => m.Id == 0).ToArray();
                AppointmentType[] editObjects = appointmentTypes.Where(m => m.Id > 0).ToArray();
                if (newObjetcs.Length > 0)
                    db.AppointmentType.AddRange(newObjetcs);
                if (editObjects.Length > 0)
                {
                    foreach (var item in editObjects)
                    {
                        var dbo = db.AppointmentType.Find(item.Id);
                        if (dbo != null)
                            //db.Customer.Attach(item);
                            //db.Entry(item).State = EntityState.Modified;
                            db.Entry(dbo).CurrentValues.SetValues(item);
                    }
                }
                db.SaveChanges();
                long[] retval = appointmentTypes.Select(m => m.Id).ToArray();
                return retval;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException exception)
            {
                string verror = "Validation Error:";
                foreach (var item in exception.EntityValidationErrors)
                {
                    verror += "\n - " + item.ValidationErrors.First().ErrorMessage;
                }

                throw new Exception(verror);
            }
            finally { db.CloseDb(); }
        }
        public void DeleteAppointmentType(long[] appointmentTypeIds)
        {
            if (appointmentTypeIds != null && appointmentTypeIds.Length > 0)
            {
                foreach (var item in appointmentTypeIds)
                {
                    AppointmentType c = db.AppointmentType.Where(m => m.Id == item).FirstOrDefault();
                    if (c == null)
                        throw new ArgumentException("Invalid Appointment Type Id");

                    db.AppointmentType.Remove(c);
                }
            }
            db.SaveChanges();
        }

        public Appointment[] GetAppointment()
        {
            db.OpenDb();
            try
            {
                return db.Appointment.ToArray();
            }
            finally { db.CloseDb(); }
        }
        public long[] SetAppointment(Appointment[] appointments)
        {
            db.OpenDb();
            try
            {
                Appointment[] newObjetcs = appointments.Where(m => m.Id == 0).ToArray();
                Appointment[] editObjects = appointments.Where(m => m.Id > 0).ToArray();
                if (newObjetcs.Length > 0)
                    db.Appointment.AddRange(newObjetcs);
                if (editObjects.Length > 0)
                {
                    foreach (var item in editObjects)
                    {
                        var dbo = db.Appointment.Find(item.Id);
                        if (dbo != null)
                            //db.Customer.Attach(item);
                            //db.Entry(item).State = EntityState.Modified;
                            db.Entry(dbo).CurrentValues.SetValues(item);
                    }
                }
                db.SaveChanges();
                long[] retval = appointments.Select(m => m.Id).ToArray();
                return retval;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException exception)
            {                
                string verror = "Validation Error:";
                foreach (var item in exception.EntityValidationErrors)
                {
                    verror += "\n - " + item.ValidationErrors.First().ErrorMessage;
                }

                throw new Exception(verror);
            }
            finally { db.CloseDb(); }
        }
        public void DeleteAppointment(long[] appointmentIds)
        {
            if (appointmentIds != null && appointmentIds.Length > 0)
            {
                foreach (var item in appointmentIds)
                {
                    Appointment c = db.Appointment.Where(m => m.Id == item).FirstOrDefault();
                    if (c == null)
                        throw new ArgumentException("Invalid Appointment Id");

                    db.Appointment.Remove(c);
                }
            }
            db.SaveChanges();
        }
        public Appointment[] GetAppointment(DateTime startDate, DateTime endDate)
        {
            db.OpenDb();
            try
            {
                DateTime dtstart = startDate.Date;
                DateTime dtend = endDate.Date.AddDays(1).AddSeconds(-1);

                return db.Appointment.Where(m => m.AppointmentDate >= dtstart && m.AppointmentDate <= dtend).ToArray();
            }
            finally { db.CloseDb(); }
        }

        public Appointment GetAppointmentById(long appId)
        {
            db.OpenDb();
            try
            {
                return db.Appointment.Where(m => m.Id == appId).FirstOrDefault();
            }
            finally { db.CloseDb(); }
        }
        public Appointment[] GetAppointmentByCustomer(long customerId)
        {
            db.OpenDb();
            try
            {
                return db.Appointment.Where(m => m.Id_Customer == customerId).ToArray();
            }
            finally { db.CloseDb(); }
        }

        public long GetAppointmentMaxNumber()
        {
            db.OpenDb();
            try
            {
                if (db.Appointment.Count() < 1)
                    return 0;
                else
                    return db.Appointment.Max(m => m.Number);
            }
            finally { db.CloseDb(); }
        }
    }
}
