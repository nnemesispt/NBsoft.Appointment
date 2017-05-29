using NBsoft.Appointment.DAL.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.WPF.ViewModels
{
    public class AppointmentVM: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        long id;
        long id_AppointmentType;
        long id_Doctor;
        long id_Customer;
        long id_User;
                
        string appointmentType;
        string customerName;
        string userName;
        string doctorName;

        string coin;
        decimal exchange;
        decimal totalProducts;
        decimal vATValue;
        decimal financeDiscount;

        public AppointmentVM()
        {
            CreationDate = DateTime.Now;
            Comments = "";    
        }
        public long Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
        public long Id_AppointmentType { get { return id_AppointmentType; } set { id_AppointmentType = value; OnPropertyChanged(nameof(Id_AppointmentType)); } }
        public long Id_Doctor { get { return id_Doctor; } set { id_Doctor = value; OnPropertyChanged(nameof(Id_Doctor)); } }
        public long Id_Customer { get { return id_Customer; } set { id_Customer = value; OnPropertyChanged(nameof(Id_Customer)); } }
        public long Id_User { get { return id_User; } set { id_User = value; OnPropertyChanged(nameof(Id_User)); } }
        public long Number { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime AppointmentDate { get; set; }
        
        public string Description { get; set; }
        //public decimal ClientDiscount { get; set; }
        public decimal FinanceDiscount
        {
            get { return financeDiscount; }
            set
            {
                financeDiscount = value;
                OnPropertyChanged(nameof(FinanceDiscount));
                OnPropertyChanged(nameof(DiscountValue));
                OnPropertyChanged(nameof(GrossValue));
                OnPropertyChanged(nameof(VAT));
                OnPropertyChanged(nameof(NetValue));
            }
        }

        

        public string PaymentType { get; set; }
        public string Coin { get { return coin; } set { coin = value; OnPropertyChanged(nameof(Coin)); } }
        public decimal Exchange { get { return exchange; } set { exchange = value; OnPropertyChanged(nameof(Exchange)); } }
        public decimal TotalProducts
        {
            get { return totalProducts; }
            set
            {
                totalProducts = value;
                OnPropertyChanged(nameof(ProductsValue));
                OnPropertyChanged(nameof(TotalProducts));
                OnPropertyChanged(nameof(DiscountValue));
                OnPropertyChanged(nameof(GrossValue));
                OnPropertyChanged(nameof(VAT));
                OnPropertyChanged(nameof(NetValue));
            }
        }
        public decimal VATValue
        {
            get { return vATValue; }
            set
            {
                vATValue = value;
                OnPropertyChanged(nameof(VATValue));
                OnPropertyChanged(nameof(VAT));
                OnPropertyChanged(nameof(NetValue));
            }
        }
        //public decimal ComercialDiscountVal { get; set; }
        public string Report { get; set; }
        public string Comments { get; set; }

        public string AppointmentType { get { return appointmentType; } set { appointmentType = value; OnPropertyChanged(nameof(AppointmentType)); } }
        public string DoctorName { get { return doctorName; } set { doctorName = value; OnPropertyChanged(nameof(DoctorName)); } }
        public string CustomerName { get { return customerName; } set { customerName = value; OnPropertyChanged(nameof(CustomerName)); } }
        public string UserName { get { return userName; } set { userName = value; OnPropertyChanged(nameof(UserName)); } }

        decimal Quantity { get { return 1; } }

        /// <summary>
        /// Value of products (Quantity * UnitPrice)
        /// </summary>
        public decimal ProductsValue { get { return TotalProducts * Quantity; } }
        /// <summary>
        /// Value of Discount
        /// </summary>
        public decimal DiscountValue { get { return ProductsValue * (FinanceDiscount / 100m); } }
        /// <summary>
        /// Gross Value (value of products - value of discount)
        /// </summary>
        public decimal GrossValue { get { return ProductsValue - DiscountValue; } }
        /// <summary>
        /// Value of VAT
        /// </summary>
        public decimal VAT { get { return GrossValue * (VATValue / 100m); } }
        /// <summary>
        /// Net value grossvalue + vatvalue
        /// </summary>
        public decimal NetValue { get { return GrossValue + VAT; } }

        public string Link { get { return string.Format("[url=/Pages/AppointmentsPage.xaml#{0}|_top]{0}[/url]", this.Id); } }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetStringId()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}",
                this.Id,
                this.Id_AppointmentType,
                this.Id_Doctor,
                this.Id_Customer,
                this.Id_User,
                this.Number,
                this.CreationDate.ToString("yyyyMMddHHmmssfff"),
                this.AppointmentDate.ToString("yyyyMMddHHmmssfff"),                
                this.Description,                
                this.FinanceDiscount,
                this.PaymentType,
                this.Coin,
                this.Exchange,
                this.TotalProducts,
                this.VATValue,
                //this.ComercialDiscountVal,
                this.Report,
                this.Comments                
                );
        }

        internal static AppointmentVM FromDBO(DAL.DataModel.Appointment a)
        {
            return FromDBO(new DAL.DataModel.Appointment[] { a }).FirstOrDefault();            
        }
        internal static AppointmentVM[] FromDBO(DAL.DataModel.Appointment[] appointments)
        {
            if (appointments == null || appointments.Length < 1)
                return new AppointmentVM[0];

            return (from a in appointments
                    select new AppointmentVM()
                    {
                        Id = a.Id,
                        Id_AppointmentType = a.Id_AppointmentType,
                        Id_Doctor = a.Id_Doctor,
                        Id_Customer = a.Id_Customer,
                        Id_User = a.Id_User,
                        Number = a.Number,
                        CreationDate = a.CreationDate,
                        AppointmentDate = a.AppointmentDate,                        
                        Description = a.Description,
                        //ClientDiscount = a.ClientDiscount,
                        FinanceDiscount = a.FinanceDiscount,
                        PaymentType = a.PaymentType,
                        Coin = a.Coin,
                        Exchange = a.Exchange,
                        TotalProducts = a.TotalProducts,
                        VATValue = a.VATValue,
                        //ComercialDiscountVal = a.ComercialDiscountVal,
                        Report = a.Report,
                        Comments = a.Comments
                    }).ToArray();


        }
    }
}
