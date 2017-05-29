using NBsoft.Appointment.WPF.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NBsoft.Appointment.WPF.Common
{
    public class NBLicense
    {
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string TaxIdNumber { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }

        public int LicenseType { get; set; }
        public bool IsRental { get; set; }
        public DateTime LicenseDate { get; set; }
        public DateTime LicenseExpiration { get; set; }

        public string UIK { get; set; }
        public Guid ServerId { get; set; }

        public LicenseType LicenseTypeC { get { return (LicenseType)LicenseType; } }

        public NBLicense()
        {
            ServerId = Guid.NewGuid();
            CreationDate = DateTime.Now;
            LicenseDate = new DateTime(2000, 1, 1);
            LicenseType = (int)Enums.LicenseType.Free;
            LicenseExpiration = DateTime.Today.AddDays(60);
            Name = "nbsoft.appointment";
            TaxIdNumber = "999999990";
            Email = "nuno.araujo@nbsoft.pt";
        }

        public string ToXml()
        {
            XmlDocument xDoc = new XmlDocument();

            XmlNode Dec = xDoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xDoc.AppendChild(Dec);

            XmlNode Root = xDoc.CreateElement("IXPLicense");
            xDoc.AppendChild(Root);

            XmlNode CreationDateNode = xDoc.CreateElement("CreationDate");
            CreationDateNode.InnerText = this.CreationDate.ToString("yyyy-MM-dd HH:mm:ss");
            Root.AppendChild(CreationDateNode);

            XmlNode NameNode = xDoc.CreateElement("Name");
            NameNode.InnerText = this.Name;
            Root.AppendChild(NameNode);

            XmlNode AddressNode = xDoc.CreateElement("Address");
            AddressNode.InnerText = this.Address;
            Root.AppendChild(AddressNode);

            XmlNode PostalCodeNode = xDoc.CreateElement("PostalCode");
            PostalCodeNode.InnerText = this.PostalCode;
            Root.AppendChild(PostalCodeNode);

            XmlNode CityNode = xDoc.CreateElement("City");
            CityNode.InnerText = this.City;
            Root.AppendChild(CityNode);

            XmlNode CountryNode = xDoc.CreateElement("Country");
            CountryNode.InnerText = this.Country;
            Root.AppendChild(CountryNode);

            XmlNode TaxIdNumberNode = xDoc.CreateElement("TaxIdNumber");
            TaxIdNumberNode.InnerText = this.TaxIdNumber;
            Root.AppendChild(TaxIdNumberNode);

            XmlNode MobileNode = xDoc.CreateElement("MobilePhone");
            MobileNode.InnerText = this.Mobile;
            Root.AppendChild(MobileNode);

            XmlNode TelephoneNode = xDoc.CreateElement("Telephone");
            TelephoneNode.InnerText = this.Phone;
            Root.AppendChild(TelephoneNode);

            XmlNode FaxNode = xDoc.CreateElement("Fax");
            FaxNode.InnerText = this.Fax;
            Root.AppendChild(FaxNode);

            XmlNode EmailNode = xDoc.CreateElement("Email");
            EmailNode.InnerText = this.Email;
            Root.AppendChild(EmailNode);

            XmlNode UrlNode = xDoc.CreateElement("Url");
            UrlNode.InnerText = this.Url;
            Root.AppendChild(UrlNode);

            XmlNode LicenseTypeNode = xDoc.CreateElement("LicenseType");
            LicenseTypeNode.InnerText = this.LicenseType.ToString();
            Root.AppendChild(LicenseTypeNode);

            XmlNode IsRentalNode = xDoc.CreateElement("IsRental");
            IsRentalNode.InnerText = this.IsRental.ToString();
            Root.AppendChild(IsRentalNode);

            XmlNode LicenseDateNode = xDoc.CreateElement("LicenseDate");
            LicenseDateNode.InnerText = this.CreationDate.ToString("yyyy-MM-dd HH:mm:ss");
            Root.AppendChild(LicenseDateNode);

            XmlNode LicenseExpirationNode = xDoc.CreateElement("LicenseExpiration");
            LicenseExpirationNode.InnerText = this.LicenseExpiration.ToString("yyyy-MM-dd HH:mm:ss");
            Root.AppendChild(LicenseExpirationNode);

            XmlNode UIKNode = xDoc.CreateElement("UIK");
            UIKNode.InnerText = this.UIK;
            Root.AppendChild(UIKNode);

            XmlNode ServerIdNode = xDoc.CreateElement("ServerId");
            ServerIdNode.InnerText = this.ServerId.ToString();
            Root.AppendChild(ServerIdNode);

            return xDoc.InnerXml;

        }

        public static NBLicense FromXml(string xml)
        {
            NBLicense retval = new NBLicense();

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);
            XmlElement Root = xDoc.DocumentElement;

            #region CreationDate
            XmlNodeList CreationDateNode = Root.GetElementsByTagName("CreationDate");
            if (CreationDateNode != null && CreationDateNode.Count > 0)
                retval.CreationDate = DateTime.Parse(CreationDateNode[0].InnerText);

            #endregion

            #region Name
            XmlNodeList NameNode = Root.GetElementsByTagName("Name");
            if (NameNode == null || NameNode.Count < 1)
                throw new ApplicationException("Invalid Configuration File");
            retval.Name = NameNode[0].InnerText;
            #endregion

            #region Address
            XmlNodeList AddressNode = Root.GetElementsByTagName("Address");
            if (AddressNode != null && AddressNode.Count > 0)
                retval.Address = AddressNode[0].InnerText;
            #endregion

            #region PostalCode
            XmlNodeList PostalCodeNode = Root.GetElementsByTagName("PostalCode");
            if (PostalCodeNode != null && PostalCodeNode.Count > 0)
                retval.PostalCode = PostalCodeNode[0].InnerText;
            #endregion

            #region City
            XmlNodeList CityNode = Root.GetElementsByTagName("City");
            if (CityNode != null && CityNode.Count > 0)
                retval.City = CityNode[0].InnerText;
            #endregion

            #region Country
            XmlNodeList CountryNode = Root.GetElementsByTagName("Country");
            if (CountryNode != null && CountryNode.Count > 0)
                retval.Country = CountryNode[0].InnerText;
            #endregion

            #region TaxIdNumber
            XmlNodeList TaxIdNumberNode = Root.GetElementsByTagName("TaxIdNumber");
            if (TaxIdNumberNode == null || TaxIdNumberNode.Count < 1)
                throw new ApplicationException("Invalid Configuration File");
            retval.TaxIdNumber = TaxIdNumberNode[0].InnerText;
            #endregion

            #region MobilePhone
            XmlNodeList MobilePhoneNode = Root.GetElementsByTagName("MobilePhone");
            if (MobilePhoneNode != null && MobilePhoneNode.Count > 0)
                retval.Mobile = MobilePhoneNode[0].InnerText;
            #endregion

            #region Telephone
            XmlNodeList TelephoneNode = Root.GetElementsByTagName("Telephone");
            if (TelephoneNode != null && TelephoneNode.Count > 0)
                retval.Phone = TelephoneNode[0].InnerText;
            #endregion

            #region Fax
            XmlNodeList FaxNode = Root.GetElementsByTagName("Fax");
            if (FaxNode != null && FaxNode.Count > 0)
                retval.Fax = FaxNode[0].InnerText;
            #endregion

            #region Email 
            XmlNodeList EmailNode = Root.GetElementsByTagName("Email");
            if (EmailNode == null || EmailNode.Count < 1)
                throw new ApplicationException("Invalid Configuration File");
            retval.Email = EmailNode[0].InnerText;
            #endregion

            #region Url
            XmlNodeList UrlNode = Root.GetElementsByTagName("Url");
            if (UrlNode != null && UrlNode.Count > 0)
                retval.Url = UrlNode[0].InnerText;
            #endregion

            #region LicenseType
            XmlNodeList LicenseTypeNode = Root.GetElementsByTagName("LicenseType");
            if (LicenseTypeNode != null && LicenseTypeNode.Count > 0)
            {
                int LT = 0;
                if (int.TryParse(LicenseTypeNode[0].InnerText, out LT))
                    retval.LicenseType = LT;
            }
            #endregion

            #region IsRental
            XmlNodeList IsRentalNode = Root.GetElementsByTagName("IsRental");
            if (IsRentalNode != null && IsRentalNode.Count > 0)
            {
                bool tmp = false;
                bool.TryParse(IsRentalNode[0].InnerText, out tmp);
                retval.IsRental = tmp;
            }
            #endregion

            #region LicenseDate
            XmlNodeList LicenseDateNode = Root.GetElementsByTagName("LicenseDate");
            if (LicenseDateNode != null && LicenseDateNode.Count > 0)
            {
                DateTime ld;
                if (DateTime.TryParse(LicenseDateNode[0].InnerText, out ld))
                    retval.LicenseDate = ld;
            }
            #endregion

            #region LicenseExpiration
            XmlNodeList LicenseExpirationNode = Root.GetElementsByTagName("LicenseExpiration");
            if (LicenseExpirationNode != null && LicenseExpirationNode.Count > 0)
            {
                DateTime ld;
                DateTime.TryParse(LicenseExpirationNode[0].InnerText, out ld);
                retval.LicenseExpiration = ld;
            }
            #endregion

            #region UIK
            XmlNodeList UIKNode = Root.GetElementsByTagName("UIK");
            if (UIKNode != null && UIKNode.Count > 0)
                retval.UIK = UIKNode[0].InnerText;
            #endregion

            #region Server Id
            XmlNodeList ServerIdNode = Root.GetElementsByTagName("ServerId");
            if (ServerIdNode != null && ServerIdNode.Count > 0)
            {
                Guid tmp = Guid.Empty;
                Guid.TryParse(ServerIdNode[0].InnerText, out tmp);
                retval.ServerId = tmp;
            }
            #endregion

            return retval;
        }
    }
}
