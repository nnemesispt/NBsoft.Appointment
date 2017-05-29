using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.Common
{
    public class LicenseEventArgs : System.EventArgs
    {
        public NBLicense License { get; private set; }
        public LicenseEventArgs(NBLicense license)
        {
            License = license;
        }
    }
}
