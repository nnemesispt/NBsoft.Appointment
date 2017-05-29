using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.ViewModels
{
    public class UserVM
    {
        public UserVM()
        {            
            Account = "newuser";            
        }

        public long Id { get; set; }
        public string Account { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Name { get { return Account; } }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
        public int PIN { get; set; }
        public string Email { get; set; }

    }
}
