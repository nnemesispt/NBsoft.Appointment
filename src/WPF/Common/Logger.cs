using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static NBsoft.Appointment.WPF.Globals;

namespace NBsoft.Appointment.WPF.Common
{
    public class Logger : IDisposable
    {
        

        private string _FolderName;
        private Mutex _LogMutex = new Mutex();
        private List<string> _LogQueue;
        private int _MaxLogEntries;


        public Logger(string Directory)
        {

            _FolderName = Directory;
            _LogQueue = new List<string>(512);
            if (!System.IO.Directory.Exists(Directory))
            {
                try { System.IO.Directory.CreateDirectory(Directory); }
                catch { throw; }
            }
            _MaxLogEntries = 4;
        }
        ~Logger()
        {
            FlushLogs();
        }

        public int MaxLogEntries
        {
            get { return _MaxLogEntries; }
            set { _MaxLogEntries = value; }
        }

        public void Log(object sender, Enums.LogType type, string LogString)
        {
            LogEventArgs e = new LogEventArgs(sender, LogString, type);
            OnLogEvent(e);
        }

        /// <summary>
        /// Raises LogEntryAdded event.
        /// </summary>
        /// <param name="e">Log entry details</param>
        protected void OnLogEvent(LogEventArgs e)
        {
            LogEvent?.Invoke(this, e);
            if (e.Cancel)
                return;

            string LogLine = string.Format("{0};{1};{2};{3}",
                        e.LogDate.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                        e.Sender,
                        e.LogType,
                        e.LogText);
            _LogMutex.WaitOne();
            _LogQueue.Add(LogLine);
            _LogMutex.ReleaseMutex();
            if (_LogQueue.Count >= _MaxLogEntries)
                FlushLogs();
        }
        public void Flush()
        {
            FlushLogs();
        }
        private void FlushLogs()
        {
            if (_LogQueue.Count < 1)
                return;

            _LogMutex.WaitOne();
            Queue<string> tmp = new Queue<string>(_LogQueue);
            _LogQueue.Clear();

            string m_LogFile = string.Format("{0}{1}{2}.log",
                _FolderName,
                Path.DirectorySeparatorChar,
                DateTime.Today.ToString("yyyy-MM-dd"));
            try
            {
                using (StreamWriter sw = new StreamWriter(m_LogFile, true))
                {
                    while (tmp.Count > 0)
                    {
                        string l = tmp.Dequeue();
                        sw.WriteLine(l);
                    }
                    sw.Close();
                }
            }
            catch (Exception ex01)
            {
                Console.WriteLine("Log Fail: {0}\nStack: [{1}]", ex01.Message, ex01.StackTrace);

            }
            finally
            {
                try { _LogMutex.ReleaseMutex(); }
                catch { }
            }
        }


        public event LogEventHandler LogEvent;



        #region IDisposable Members

        public void Dispose()
        {
            FlushLogs();
        }

        #endregion

    }
}
