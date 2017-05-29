using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace NBsoft.Appointment.DAL.DataModel
{
    public partial class AppointmentEntities:DbContext
    {
        public AppointmentEntities(string connectionString) 
            : base(connectionString)
        {

        }
        public void OpenDb()
        {
            this.Database.Connection.Open();
        }
        public void CloseDb()
        {
            this.Database.Connection.Close();
        }
    }
}
