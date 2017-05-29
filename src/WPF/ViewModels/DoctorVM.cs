using NBsoft.Appointment.DAL.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.ViewModels
{
    public class DoctorVM
    {
        public DoctorVM()
        {
            CreationDate = DateTime.Now;
            Name = "name";
            TaxIdNumber = "999 999 990";
            
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
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
        public string Comments { get; set; }
        public string Contact { get; set; }

        public string GetStringId()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}",
                this.Id,                
                this.CreationDate.ToString("yyyyMMddHHmmssfff"),
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
                this.Comments,
                this.Contact
                );
        }

        internal static DoctorVM FromDBO(Doctor c)
        {
            return new DoctorVM()
            {
                Address = c.Address,
                City = c.City,
                Contact = c.Contact,
                Country = c.Country,
                CreationDate = c.CreationDate,                
                EMail = c.EMail,
                Fax = c.Fax,
                IBAN = c.IBAN,
                Id = c.Id,                
                MobilePhone = c.MobilePhone,
                Name = c.Name,
                PostalCode = c.PostalCode,
                TaxIdNumber = c.TaxIdNumber,
                Telephone = c.Telephone,
                URL = c.URL,
                Comments = c.Comments
            };

        }
    }
}
