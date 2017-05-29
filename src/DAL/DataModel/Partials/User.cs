using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.DAL.DataModel
{
    public partial class User
    {
        public string Name
        {
            get
            {
                string retval = string.IsNullOrEmpty(Firstname) ? "" : Firstname + " ";
                retval += string.IsNullOrEmpty(Lastname) ? "" : Lastname + " ";
                retval += (retval.Length > 0) ? "" : Logon;
                return retval.TrimEnd();
            }
        }
    }
}
