using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.Common
{
    public class LogEventArgs : System.EventArgs
    {
        DateTime _LogDate;
        Enums.LogType _LogType;
        string _LogText;
        bool _Cancel;
        object _Sender;


        /// <summary>
        /// Creates a new instance of LogEventArgs class
        /// </summary>
        public LogEventArgs()
        {
            _LogDate = DateTime.Now;
            _LogType = Enums.LogType.Info;
            _LogText = "";
            _Cancel = false;
        }
        public LogEventArgs(object sender, string logText)
            : this()
        {
            _LogText = logText;
            _Sender = sender;
        }
        public LogEventArgs(object sender, string logText, Enums.LogType logType)
            : this()
        {
            _LogType = logType;
            _LogText = logText;
            _Sender = sender;
        }

        /// <summary>
        /// Log entry date
        /// </summary>
        public DateTime LogDate
        {
            get { return _LogDate; }
            set { _LogDate = value; }
        }

        /// <summary>
        /// Log entry type
        /// </summary>
        public Enums.LogType LogType
        {
            get { return _LogType; }
            set { _LogType = value; }
        }

        /// <summary>
        /// Log entry text
        /// </summary>
        public string LogText
        {
            get { return _LogText; }
            set { _LogText = value; }
        }
        /// <summary>
        /// Cancels log entry.
        /// </summary>
        public bool Cancel
        {
            get { return _Cancel; }
            set { _Cancel = value; }
        }

        /// <summary>
        /// Object that originated the log entry.
        /// </summary>
        public object Sender
        {
            get { return _Sender; }
            set { _Sender = value; }
        }
    }
}
