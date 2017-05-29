using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.Dictionary
{    
    public class LanguageChangedEventArgs : System.EventArgs
    {
        Language _newlang;

        public LanguageChangedEventArgs(Language language)
        {
            _newlang = language;
        }

        public Language Language { get { return _newlang; } set { _newlang = value; } }
    }

    public delegate void LanguageChangedDelegate(object sender, LanguageChangedEventArgs e);
}
