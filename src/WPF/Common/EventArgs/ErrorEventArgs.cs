using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.Common
{
    public class ErrorEventArgs : System.EventArgs
    {
        Exception _exception;
        string _senderMethod;
        public ErrorEventArgs()
        {
            _exception = null;
            _senderMethod = "";
        }
        public ErrorEventArgs(Exception Exception) : this()
        {
            _exception = Exception;
            _senderMethod = "";
        }
        public ErrorEventArgs(Exception Exception, string SenderMethod)
            : this()
        {
            _exception = Exception;
            _senderMethod = SenderMethod;
        }
        public Exception Exception { get { return _exception; } set { _exception = value; } }
        public string SenderMethod { get { return _senderMethod; } set { _senderMethod = value; } }

    }
}
