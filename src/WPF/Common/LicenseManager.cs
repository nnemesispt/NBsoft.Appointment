using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.Common
{
    public class LicenseManager
    {
        const long LicenseAppId = 2;
        public event ErrorEventHandler CheckLicenseError;
        public event LicenseEventHandler CheckLicenseFinished;

        public Uri LicenseServerEndPoint { get; private set; }
        string LicenseFile;
        //IXPLicense LocalLicense;

        public NBLicense ServerLicense { get; private set; }
        private string LastConnFile { get { return Globals.CommonPath + "\\_lc.bin"; } }

        public LicenseManager(Uri LicenseServerUrl)
        {
            LicenseServerEndPoint = LicenseServerUrl;

            System.IO.FileInfo fi = new System.IO.FileInfo(LastConnFile);
            if (!fi.Exists)
                SetLastConnection(new DateTime(2000, 1, 1));
        }


        /// <summary>
        /// Check Current Server License with License Server
        /// </summary>        
        /// <param name="ServerId">Unique Server Id</param>
        /// <param name="TaxIdNumber"></param>
        /// <param name="CompanyName"></param>
        /// <param name="Email"></param>
        /// <param name="UIK">Unique Identification Key</param>
        /// <returns>License information or NULL if not licensed</returns>
        public async Task<NBLicense> ValidateLicense(Guid ServerId, string TaxIdNumber, string CompanyName, string Email, string UIK)
        {

            string html = string.Empty;

            string uripath = LicenseServerEndPoint.ToString();
            if (!uripath.EndsWith("/"))
                uripath += "/";
            string url = string.Format("{0}Companies/CheckCompanyServer?ServerId={1}", uripath, ServerId);
            // lic2.0
            //url = string.Format("{0}Licenses/CheckLicense?serverId={1}&companytoken={2}&huik={3}&appId={4}", uripath, ServerId,TaxIdNumber, UIK, LicenseAppId);
            //html = GetHttpRequest(url);

            // lic2.1
            url = string.Format("{0}Licenses/CheckLicense", uripath);
            Dictionary<string, string> CheckLicensePars = new Dictionary<string, string>
            {
                {"serverId", string.Format("{0}", ServerId) },
                {"companytoken", string.Format("{0}", TaxIdNumber) },
                {"huik", string.Format("{0}", UIK) },
                {"appId", string.Format("{0}", LicenseAppId) },
                {"validationtoken", string.Format("{0}", Globals.AppGuid) }
            };
            html = await SendHttpPostAsync(url, CheckLicensePars);



            if (html == "Invalid Server Id")
            {
                // Server not registered, register InvXPress Server
                //string registerUrl = string.Format("{0}Licenses/Register?serverId={1}&companytoken={2}&companyName={3}&email={4}&huik={5}&appId={6}", uripath, ServerId, TaxIdNumber, CompanyName, Email, UIK, LicenseAppId);
                //html = GetHttpRequest(registerUrl);

                // lic2.1
                string registerUrl = string.Format("{0}Licenses/Register", uripath);
                Dictionary<string, string> RegisterPars = new Dictionary<string, string>
                {
                    {"serverId", string.Format("{0}", ServerId) },
                    {"companytoken", string.Format("{0}", TaxIdNumber) },
                    {"companyName", string.Format("{0}", CompanyName) },
                    {"email", string.Format("{0}", Email) },
                    {"huik", string.Format("{0}", UIK) },
                    {"appId", string.Format("{0}", LicenseAppId) },
                    {"validationtoken", string.Format("{0}", Globals.AppGuid) }
                };
                html = await SendHttpPostAsync(registerUrl, RegisterPars);


                if (!html.StartsWith("<?xml"))
                    throw new ApplicationException(html);

                //html = GetHttpRequest(url);
                html = await SendHttpPostAsync(url, CheckLicensePars);
            }
            if (!html.StartsWith("<?xml"))
                throw new ApplicationException(html);

            NBLicense retval = NBLicense.FromXml(html);
            if (retval.UIK != UIK)
                throw new ApplicationException("Invalid UIK");
            return retval;
        }

        public void CheckLicenseAsync(string licenseFile, string UIK)
        {
            if (licenseFile == null || licenseFile == "")
                throw new ArgumentNullException("licenseFile");

            System.IO.FileInfo LFile = new System.IO.FileInfo(licenseFile);
            if (!LFile.Exists)
                throw new System.IO.FileNotFoundException(licenseFile);

            LicenseFile = licenseFile;

            BackgroundWorker CheckLicenseBW = new BackgroundWorker();
            CheckLicenseBW.DoWork += CheckLicenseBW_DoWork;
            CheckLicenseBW.RunWorkerCompleted += CheckLicenseBW_RunWorkerCompleted;
            CheckLicenseBW.RunWorkerAsync(UIK);
        }
        void CheckLicenseBW_DoWork(object sender, DoWorkEventArgs e)
        {
            string uik = e.Argument.ToString();
            string Xml = "";
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(LicenseFile, Encoding.UTF8, false))
                {
                    Xml = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch { Xml = ""; }
            NBLicense localLic;
            if (Xml == "")
                localLic = new Common.NBLicense();
            else
                localLic = NBLicense.FromXml(Xml);
            Common.NBLicense remoteLicense;
            try
            {
                Task<Common.NBLicense> res = ValidateLicense(localLic.ServerId, localLic.TaxIdNumber, localLic.Name, localLic.Email, uik);
                while (!res.IsCompleted)
                {
                    System.Threading.Thread.Sleep(50);

                }
                remoteLicense = res.Result;
            }
            catch (Exception ex01)
            {
                if (ex01.InnerException != null)
                    throw ex01.InnerException;
                else
                    throw;

            }
            if (remoteLicense == null)
            {
                // No License check if demo mode has finished, 
                // If Demo has finished, apply free license
                // Demo is still running, apply demo license


                remoteLicense = new Common.NBLicense()
                {
                    TaxIdNumber = localLic.TaxIdNumber,
                    Email = localLic.Email,
                    Name = localLic.Name,
                    ServerId = localLic.ServerId
                };

                DateTime installdate = Globals.GetInstallDate();
                remoteLicense.LicenseDate = installdate;
                remoteLicense.LicenseExpiration = installdate.AddDays(60);

                if (DateTime.Now > remoteLicense.LicenseExpiration)
                    remoteLicense.LicenseType = (int)Enums.LicenseType.Free;                

                ServerLicense = remoteLicense;
            }
            else
            {
                // License is OK
                // Update Local License                
                ServerLicense = remoteLicense;

            }

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(LicenseFile, false, Encoding.UTF8))
            {
                sw.Write(ServerLicense.ToXml());
                sw.Close();
            }
            if (remoteLicense.LicenseExpiration < DateTime.Now)
                throw new ApplicationException("License Expired. Contact Support");

        }
        void CheckLicenseBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ServerLicense = null;
                DateTime LastConnection = GetLastConnection();
                if (LastConnection.AddDays(30) < DateTime.Now)                
                    OnCheckLicenseError(new ErrorEventArgs(new ApplicationException("License Check Timelimit Hit")));                
                else
                    OnCheckLicenseError(new ErrorEventArgs(e.Error));
                return;
            }
            DateTime lastconn = DateTime.Now;
            //test
            //lastconn = DateTime.Now.AddMonths(-6);
            SetLastConnection(lastconn);
            OnCheckLicenseFinished(new LicenseEventArgs(ServerLicense));
        }

        public async Task<NBLicense> UpdateLicense(NBLicense license)
        {
            string html = string.Empty;

            string uripath = LicenseServerEndPoint.ToString();
            if (!uripath.EndsWith("/"))
                uripath += "/";

            string contacts = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                license.Email,
                license.Phone,
                license.Mobile,
                license.Fax,
                license.Url,
                license.Address,
                license.PostalCode,
                license.City,
                license.Country
                );

            // lic2.0
            //string url = string.Format("{0}Licenses/UserUpdate?serverId={1}&appId={2}&companyName={3}&taxIdNumber={4}&contacts={5}", uripath, license.ServerId, LicenseAppId, license.Name, license.TaxIdNumber, contacts);
            //html = GetHttpRequest(url);

            // lic2.1
            string url = string.Format("{0}Licenses/UserUpdate", uripath);
            Dictionary<string, string> CheckLicensePars = new Dictionary<string, string>
            {
                {"serverId", string.Format("{0}", license.ServerId) },
                {"appId", string.Format("{0}", LicenseAppId) },
                {"companyName", string.Format("{0}", license.Name) },
                {"taxIdNumber", string.Format("{0}", license.TaxIdNumber) },
                {"contacts", string.Format("{0}", contacts) },
                {"validationtoken", string.Format("{0}", Globals.AppGuid) }
            };
            html = await SendHttpPostAsync(url, CheckLicensePars);


            if (!html.StartsWith("<?xml"))
                throw new ApplicationException(html);

            NBLicense retval = NBLicense.FromXml(html);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(LicenseFile, false, Encoding.UTF8))
            {
                sw.Write(html);
                sw.Close();
            }
            return retval;
        }

        private void SetLastConnection(DateTime date)
        {
            string dtstring = Globals.Encrypt(date.ToString("yyyy-MM-dd HH:mm:ss"));
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(LastConnFile, false, Encoding.UTF8))
            {
                sw.Write(dtstring);
                sw.Close();
            }

        }
        private DateTime GetLastConnection()
        {
            string dtstring = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(LastConnFile, Encoding.UTF8, false))
            {
                dtstring = sr.ReadToEnd();
                sr.Close();
            }
            DateTime retval;
            try
            {
                string plaindt = Globals.Decrypt(dtstring);
                retval = DateTime.Parse(plaindt);
            }
            catch
            {
                retval = new DateTime(2000, 1, 1);
            }
            return retval;
        }

        void OnCheckLicenseError(ErrorEventArgs e)
        {
            CheckLicenseError?.Invoke(this, e);
        }
        void OnCheckLicenseFinished(LicenseEventArgs e)
        {
            CheckLicenseFinished?.Invoke(this, e);
        }

        private static async Task<string> SendHttpPostAsync(string url, Dictionary<string, string> postParameters)
        {
            var content = new System.Net.Http.FormUrlEncodedContent(postParameters);
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                var response = await client.PostAsync(url, content);
                var res = await response.Content.ReadAsStringAsync();
                return res;
            }
        }
       
    }
}
