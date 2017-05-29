using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBsoft.Appointment.DAL.DataModel;

namespace NBsoft.Appointment.WPF.ViewModels
{
    public class AppointmentTypeVM
    {
        public AppointmentTypeVM()
        {
            CreationDate = DateTime.Now;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public System.DateTime CreationDate { get; set; }

        public string GetStringId()
        {
            return string.Format("{0}|{1}|{2}",
                this.Id,                
                this.CreationDate.ToString("yyyyMMddHHmmssfff"),
                this.Name
                );
        }

        internal static AppointmentTypeVM FromDBO(AppointmentType item)
        {
            return new AppointmentTypeVM()
            {
                Id = item.Id,
                CreationDate = item.CreationDate,
                Name = item.Name
            };
        }
    }
}
