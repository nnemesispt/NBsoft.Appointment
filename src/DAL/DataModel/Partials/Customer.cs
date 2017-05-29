using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.DAL.DataModel
{
    public partial class Customer
    {
        public int Age
        {
            get
            {
                
                DateTime zeroTime = new DateTime(1, 1, 1);
                TimeSpan span = (DateTime.Today - this.BirthDate);
                int years = (zeroTime + span).Year - 1;
                return years;
            }
        }

        public string LastAppointment { get; set; }

        public string Comment1Line { get { return this.Comments.Replace("\n", " | "); } }

    }
}
