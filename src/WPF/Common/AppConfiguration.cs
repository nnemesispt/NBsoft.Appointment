using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NBsoft.Appointment.WPF.Common
{
    public class AppConfiguration
    {
        public AppConfiguration()
        {
            TerminalId = Guid.NewGuid().ToString();
            MainDatabase = "";
            LicenseUrl = "http://nnemesis.myvnc.com/nbsoftapp/";
        }
        public string TerminalId { get; set; }
        public string MainDatabase { get; set; }
        public string LicenseUrl { get; set; }

        public string ToXml()
        {
            XmlDocument xDoc = new XmlDocument();

            XmlNode Dec = xDoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xDoc.AppendChild(Dec);

            XmlNode Root = xDoc.CreateElement("LocalConfig");
            xDoc.AppendChild(Root);

            XmlNode TerminalIdNode = xDoc.CreateElement("TerminalId");
            TerminalIdNode.InnerText = TerminalId;
            Root.AppendChild(TerminalIdNode);

            XmlNode MainDatabaseNode = xDoc.CreateElement("MainDatabase");
            MainDatabaseNode.InnerText = MainDatabase;
            Root.AppendChild(MainDatabaseNode);

            XmlNode LicenseUrlNode = xDoc.CreateElement("LicenseUrl");
            LicenseUrlNode.InnerText = LicenseUrl;
            Root.AppendChild(LicenseUrlNode);

            return xDoc.InnerXml;
        }

        public static AppConfiguration FromXml(string Xml)
        {

            AppConfiguration retval = new AppConfiguration();

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(Xml);
            XmlElement Root = xDoc.DocumentElement;
            
            #region TerminalId
            XmlNodeList TerminalIdNode = Root.GetElementsByTagName("TerminalId");
            if (TerminalIdNode == null || TerminalIdNode.Count < 1)
                throw new ApplicationException("Invalid Configuration File");
            retval.TerminalId = TerminalIdNode[0].InnerText;
            #endregion

            #region MainDatabase
            XmlNodeList MainDatabaseNode = Root.GetElementsByTagName("MainDatabase");
            if (MainDatabaseNode == null || MainDatabaseNode.Count < 1)
                throw new ApplicationException("Invalid Configuration File");
            retval.MainDatabase = MainDatabaseNode[0].InnerText;
            #endregion

            #region LicenseUrl
            XmlNodeList LicenseUrlNode = Root.GetElementsByTagName("LicenseUrl");
            if (LicenseUrlNode == null || LicenseUrlNode.Count < 1)
                throw new ApplicationException("Invalid Configuration File");
            retval.LicenseUrl = LicenseUrlNode[0].InnerText;
            #endregion

            return retval;
        }
    }
}
