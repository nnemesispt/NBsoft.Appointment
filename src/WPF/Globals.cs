
using NBsoft.Appointment.WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NBsoft.Appointment.WPF
{
    public delegate void LogEventHandler(object sender, LogEventArgs e);
    public delegate void LicenseEventHandler(object sender, LicenseEventArgs e);
    public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

    public static class Globals
    {
        internal const string NBASoftKey = "#TTna@2016";
        private const string Salt = ("E66CD3E5B1CF");
        private const string IV = ("BA8E98B7C5A5310F");

        private static Dictionary.Manager dicMan = null;
        private static Logger logger;

        public static Guid AppGuid = new Guid("CFA4837F-14B6-452A-A695-A13AD11B2501");

        static LicenseManager LicenseMan;
        public static NBLicense License { get; private set; }

        internal static string AppName { get { return string.Format("NBsoft.Appointment {0}", System.Reflection.Assembly.GetEntryAssembly().GetName().Version); } }
        internal static string CommonPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\NBsoft\Appointment1.0"; } }
        internal static string InstallPath { get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); } }
        internal static string ConfigurationFile { get { return CommonPath + @"\Config.xml"; } }
        internal static Logger Logger { get { return logger; } }
        internal static Windows.MainWindow MainWnd { get; private set; }
        internal static AppConfiguration LocalConfig { get; private set; }

        internal static DAL.DataModel.Database Db { get; private set; }
        internal static DAL.DataModel.MainOptions Options { get; private set; }
        internal static DAL.DataModel.User LoggedUser { get; private set; }
        

        internal static Dictionary.Manager DicMan { get { return dicMan; } }

        internal static void Startup()
        {
            // Load Configuration
            System.IO.FileInfo CfgFile = new System.IO.FileInfo(ConfigurationFile);
            if (!CfgFile.Exists)
            {
                if (!CfgFile.Directory.Exists)
                    CfgFile.Directory.Create();

                LocalConfig = new AppConfiguration();
                LocalConfig.MainDatabase = CommonPath + "\\nbsoft.appointment";
                string xml = LocalConfig.ToXml();
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(CfgFile.FullName, false, Encoding.UTF8))
                {
                    sw.Write(xml);
                    sw.Close();
                }
            }
            else
            {
                string xml;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(CfgFile.FullName, Encoding.UTF8))
                {
                    xml = sr.ReadToEnd();
                    sr.Close();
                }
                LocalConfig = AppConfiguration.FromXml(xml);
            }

            System.IO.FileInfo dbFile = new System.IO.FileInfo(LocalConfig.MainDatabase);

            string dbFolder = dbFile.Directory.FullName;
            string dbFileName  = dbFile.Name;

            //Open database
            DAL.Maintenance.ProgressChanged += Maintenance_ProgressChanged;
            DAL.Maintenance.UpdateFinished += Maintenance_UpdateFinished;

            bool CompanyDbExists = DAL.Maintenance.SQLite.CheckDb(dbFileName, dbFolder);
            if (!CompanyDbExists)
                DAL.Maintenance.SQLite.CreateDb(dbFileName, dbFolder);
            DAL.Maintenance.SQLite.UpdateDb(dbFileName, dbFolder);

            DAL.Maintenance.ProgressChanged -= Maintenance_ProgressChanged;
            DAL.Maintenance.UpdateFinished -= Maintenance_UpdateFinished;
            Db = new DAL.DataModel.Database(dbFile.FullName + ".sqlite");

            // Get Options
            Options = Db.GetOptions();

            // Check if Admin User Exists            
            var adminUser = Db.GetUser().Where(m => m.Logon == "sa");
            if (adminUser.Count() < 1)
            {
                DAL.DataModel.User saUser = new DAL.DataModel.User()
                {
                    Firstname = "Sys",
                    Lastname = "Admin",
                    Logon = "sa",
                    CreationDate = DateTime.Now,
                    Email = "geral@nbsoft.pt",
                    Language = "pt-PT",
                    PIN = "1234",
                    Password = Encrypt("sa123456"),
                    Country = "PT",
                    Theme = "dark",
                    Accentcolor = -16741888,


                };
                long[] ids = Db.SetUser(new DAL.DataModel.User[] { saUser });
            }

            // Start Dictionary
            string LangDir = InstallPath + @"\Dictionary";
            dicMan = new Dictionary.Manager(LangDir);

            // License
            string lic = System.IO.Path.Combine(Globals.CommonPath, "License.xml");
            System.IO.FileInfo fi = new System.IO.FileInfo(lic);
            if (!fi.Exists)
            {
                NBLicense l = new NBLicense();
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(lic, false, Encoding.UTF8))
                {
                    sw.Write(l.ToXml());
                    sw.Close();
                }

            }

            string HardwareUIK = HardwareKey.GetUIK();
            Uri LicenseServer = new Uri("http://nbsoft.pt/V1/");
//#if DEBUG
//            LicenseServer = new Uri("http://localhost:63787/");
//#endif
            LicenseMan = new LicenseManager(LicenseServer);
            LicenseMan.CheckLicenseError += LicenseMan_CheckLicenseError;
            LicenseMan.CheckLicenseFinished += LicenseMan_CheckLicenseFinished;
            LicenseMan.CheckLicenseAsync(System.IO.Path.Combine(lic), HardwareUIK);


            // Logon
            LoggedUser = Windows.LogonWindow.Authenticate(null, Db.GetUser());
            if (LoggedUser == null)
                System.Windows.Application.Current.Shutdown();
            else
            {
                // Start Main Window
                MainWnd = new Windows.MainWindow();
                MainWnd.Show();
            }


        }

        private static void LicenseMan_CheckLicenseError(object sender, ErrorEventArgs e)
        {
            Globals.LogError(e.Exception);

            if (e.Exception.Message == "License Check Timelimit Hit")
            {
                // Maximum Timelimit without license Check reached.
                License = null;
            }
            else
            {
                // Use Local License
                string Xml = "";
                try
                {
                    string lic = System.IO.Path.Combine(Globals.CommonPath, "License.xml");
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(lic, Encoding.UTF8, false))
                    {
                        Xml = sr.ReadToEnd();
                        sr.Close();
                    }
                }
                catch { Xml = ""; }
                License = NBLicense.FromXml(Xml);
            }
        }

        private static void LicenseMan_CheckLicenseFinished(object sender, LicenseEventArgs e)
        {
            // License is OK
            License = e.License;
            Globals.Log("LicenseManager", "License check OK: {0}", e.License.Name);
        }

        private static void Maintenance_UpdateFinished(object sender, System.EventArgs e)
        {
            Log("DAL.Maintenance", "<<DB>> UPDATE FINISHED!");
        }

        private static void Maintenance_ProgressChanged(object sender, DAL.Maintenance.ProgressEventArgs e)
        {
            Log("DAL.Maintenance", "<<DB>>[{0:F2}/{1:F2}] > {2}", e.CurrentValue, e.MaxValue, e.Status);
        }

        internal static void Log(object logsender, Enums.LogType logtype, string logtext, params object[] logParameters)
        {
            //logger.Log(LogSender, string.Format("[{0}]", LogType), Text);
            if (logger == null)
            {
                try
                {
                    logger = new Common.Logger(CommonPath + @"\Logs");
                    logger.MaxLogEntries = 16;
                    logger.LogEvent += Logger_LogEvent;
                }
                catch
                {
                    logger = null;
                    return;
                }
            }
            logger.Log(logsender, logtype, string.Format(logtext, logParameters));
            if (logtype != Enums.LogType.Info)
                logger.Flush();
        }
        internal static void Log(object logsender, string logtext, params object[] logParameters)
        {
            Log(logsender, Enums.LogType.Info, logtext, logParameters);
        }
        public static void LogError(Exception ex01)
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            string Where = st.GetFrame(1).GetMethod().Name;
            LogError(Where, ex01);
        }

        public static DateTime GetInstallDate()
        {
            string DateKey = "SOFTWARE\\NBSOFT\\Appointment1.0\\Settings";
            Microsoft.Win32.RegistryKey newKey;
            if (System.Environment.Is64BitOperatingSystem)
            {
                Microsoft.Win32.RegistryKey RegLMBase64 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
                newKey = RegLMBase64.OpenSubKey(DateKey);
            }
            else
            {
                Microsoft.Win32.RegistryKey RegLMBase32 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
                newKey = RegLMBase32.OpenSubKey(DateKey);
            }


            string value;
            try
            {
                value = newKey.GetValue("InstallDate").ToString();
            }
            catch
            {
                value = "2012-01-01";
                //try { value = newKey.GetValue("InstallDate").ToString(); }
                //catch
                //{
                //    value = "2012-01-01";
                //    //Log("GetInstallDate", Common.Logger.LogType.Error, string.Format("{0} - [{1}]", ex01.Message, ex01.StackTrace));
                //}
            }

            DateTime dt = DateTime.Parse(value);
            return dt;
        }

        public static void LogError(object sender, Exception ex01)
        {
            string msg = ex01.InnerException == null ? ex01.Message : ex01.InnerException.Message;
            string stack = ex01.StackTrace;

            Log(sender, string.Format("{0} STACK:[{1}]", msg, stack), Enums.LogType.Error);
            FlushLogs();
        }
        internal static void FlushLogs()
        {
            if (logger != null)
                logger.Flush();
        }

        public static string Encrypt(string plainText)
        {
            return Encrypt(plainText, Salt);
        }
        public static string Encrypt(string plainText, string salt)
        {
            return RijndaelSimple.Encrypt(plainText, NBASoftKey, salt, "SHA1", 1, IV, 128);
        }
        public static string Decrypt(string encryptedText)
        {
            return Decrypt(encryptedText, Salt);
        }
        public static string Decrypt(string encryptedText, string salt)
        {
            return RijndaelSimple.Decrypt(encryptedText, NBASoftKey, salt, "SHA1", 1, IV, 128);
        }

        /// <summary>
        /// Converte um numero inteiro numa cor
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static System.Windows.Media.Color GetColorFromInt(int col)
        {
            byte[] bbb = BitConverter.GetBytes(col);
            System.Windows.Media.Color c2 = System.Windows.Media.Color.FromArgb(bbb[3], bbb[0], bbb[1], bbb[2]);
            return c2;
        }
        /// <summary>
        /// Converte uma cor num inteiro
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int GetIntFromColor(System.Windows.Media.Color color)
        {
            int colorCode = BitConverter.ToInt32(new byte[] { color.R, color.G, color.B, color.A }, 0);
            return colorCode;
        }

        public static System.Windows.Media.ImageSource BitmapFromUri(Uri source)
        {
            if (source == null)
                return null;
            var bitmap = new System.Windows.Media.Imaging.BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = source;
            bitmap.EndInit();
            return bitmap;
        }

        public static DAL.DataModel.Coin[] GetCoinTable()
        {
            List<DAL.DataModel.Coin> retval = new List<DAL.DataModel.Coin>();
            retval.Add(new DAL.DataModel.Coin { Reference = "EUR", Name = "Euro", Exchange = 1 });
            retval.Add(new DAL.DataModel.Coin { Reference = "GBP", Name = "Libra Esterlina", Exchange = 1.1687m });
            return retval.ToArray();
        }
        public static DAL.DataModel.PaymentType[] GetPaymentTypeTable()
        {
            List<DAL.DataModel.PaymentType> retval = new List<DAL.DataModel.PaymentType>();
            retval.Add(new DAL.DataModel.PaymentType { Reference = "NUM", Name = "Numerário" });
            retval.Add(new DAL.DataModel.PaymentType { Reference = "MB", Name = "Multibanco" });
            retval.Add(new DAL.DataModel.PaymentType { Reference = "CHQ", Name = "Cheque" });
            retval.Add(new DAL.DataModel.PaymentType { Reference = "TRF", Name = "Transferência" });
            return retval.ToArray();
        }

        private static void Logger_LogEvent(object sender, LogEventArgs e)
        {
            Console.WriteLine("{0}|{1,7}|{2}|{3}", e.LogDate.ToString("HH:mm:ss.fff"), e.LogType, e.Sender, e.LogText);
        }

        
    }
}
