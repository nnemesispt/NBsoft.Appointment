using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBsoft.Appointment.DAL.DataModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NBsoft.Appointment.WPF.ViewModels
{
    public class CustomerVM: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime? nextAppointment;

        public CustomerVM()
        {
            CreationDate = DateTime.Now;
            Name = "";
            TaxIdNumber = "999 999 990";
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime BirthDate { get; set; }
        public string TaxIdNumber { get; set; }
        public string MobilePhone { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string EMail { get; set; }
        public string URL { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string IBAN { get; set; }
        public string Contact { get; set; }
        public string DrivingLicense { get; set; }
        public string DrivingLicenseType { get; set; }
        public DateTime DrivingLicenseDate { get; set; }
        public string IntegrationRef { get; set; }
        public DateTime? IntegrationDate { get; set; }
        public string Comments { get; set; }

        public DateTime? NextAppointment { get { return nextAppointment; } set { nextAppointment = value; OnPropertyChanged(nameof(NextAppointment)); } }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public string GetStringId()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}",
                this.Id,
                this.IntegrationRef,
                this.IntegrationDate?.ToString("yyyyMMddHHmmssfff"),
                this.CreationDate.ToString("yyyyMMddHHmmssfff"),
                this.BirthDate.ToString("yyyyMMddHHmmssfff"),
                this.Name,
                this.TaxIdNumber,
                this.MobilePhone,
                this.Telephone,
                this.Fax,
                this.EMail,
                this.URL,
                this.Address,
                this.PostalCode,
                this.City,
                this.Country,
                this.IBAN,
                this.Contact,
                this.DrivingLicense,
                this.DrivingLicenseType,
                this.DrivingLicenseDate.ToString("yyyyMMddHHmmssfff"),
                this.Comments,
                this.NextAppointment?.ToString("yyyyMMddHHmmssfff")
                );
        }

        internal static CustomerVM FromDBO(Customer c)
        {
            return new CustomerVM()
            {
                Address = c.Address,
                BirthDate = c.BirthDate,
                City= c.City,
                Contact= c.Contact,
                Country= c.Country,
                CreationDate= c.CreationDate,
                DrivingLicense= c.DrivingLicense,
                DrivingLicenseDate= c.DrivingLicenseDate,
                DrivingLicenseType= c.DrivingLicenseType,
                EMail= c.EMail,
                Fax = c.Fax,
                IBAN = c.IBAN,
                Id = c.Id,
                IntegrationDate = c.IntegrationDate,
                IntegrationRef = c.IntegrationRef,
                MobilePhone= c.MobilePhone,
                Name = c.Name,
                PostalCode = c.PostalCode,
                TaxIdNumber = c.TaxIdNumber,
                Telephone = c.Telephone,
                URL = c.URL,
                Comments = c.Comments,
                NextAppointment = c.NextAppointment

            };
            
        }
    }
}
