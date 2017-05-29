using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.Common
{
    public class TextEventArgs : System.EventArgs
    {
        string text;
        public TextEventArgs(string text)
        {
            this.Text = text;
        }

        public string Text { get { return text; } set { text = value; } }
    }
}
