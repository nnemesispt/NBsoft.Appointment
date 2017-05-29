using FirstFloor.ModernUI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Navigation;
using NBsoft.Appointment.WPF.ViewModels;
using System.ComponentModel;
using FirstFloor.ModernUI.Windows.Controls;
using System.Runtime.CompilerServices;

namespace NBsoft.Appointment.WPF.Content
{
    /// <summary>
    /// Interaction logic for AppointmentList.xaml
    /// </summary>
    public partial class AppointmentList : UserControl, IContent
    {
        LocalVM viewModel;
        List<double> ColWidths = null;
        bool IsMouseClicked = false;

        public AppointmentList()
        {
            InitializeComponent();
            viewModel = null;
            DG1.LayoutUpdated += DG1_LayoutUpdated;
            
        }

        #region Event Handlers

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.DataContext == null)
            {
                viewModel = new LocalVM()
                {
                    StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                    EndDate = DateTime.Today.AddDays(1).AddSeconds(-1)
                };
                this.DataContext = viewModel;
                GetDataAsync();
            }
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void Tskbar_Add(object sender, EventArgs e)
        {
            Pages.AppointmentsPage.ActivePage.ShowDetails(null);
        }
        private void Taskbar_Edit(object sender, EventArgs e)
        {
            if (DG1.SelectedItem == null)
                return;

            AppointmentVM current = (AppointmentVM)DG1.SelectedItem;
            Pages.AppointmentsPage.ActivePage.ShowDetails(current);            
        }        
        private void Taskbar_Print(object sender, EventArgs e)
        {

            string file = System.IO.Path.Combine(Globals.CommonPath, string.Format("{0}.csv", DateTime.Now.ToString("yyyymmdd_hhmmss")));
            DG1.ExportUsingRefection(file);            
            System.Diagnostics.Process.Start(file);
        }

        private void DG1_LayoutUpdated(object sender, EventArgs e)
        {
            if (IsMouseClicked == false)
                return;
            if (ColWidths == null)
                return;

            bool changed = false;
            try
            {
                for (int i = 0; i < DG1.Columns.Count; i++)
                {
                    if (ColWidths[i] != DG1.Columns[i].ActualWidth)
                    {
                        changed = true;
                        ColWidths[i] = DG1.Columns[i].ActualWidth;
                    }
                }
            }
            catch { changed = true; }
            if (changed)
                SetColumnMeasures();
        }
        private void DG1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseClicked = true;
        }
        private void DG1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseClicked = false;
        }
        private void DG1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG1.SelectedItem == null)
                return;

            AppointmentVM current = (AppointmentVM)DG1.SelectedItem;
            Pages.AppointmentsPage.ActivePage.SetSelection(current);
        }

        private void BtnResetCols_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < DG1.Columns.Count; i++)
            {
                DG1.Columns[i].Width = new DataGridLength(100, DataGridLengthUnitType.Auto, 100, 100);
            }
            SetColumnMeasures();
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            Pages.AppointmentsPage.ActivePage.SetSelection(null);
            GetDataAsync();
        }
        #endregion

        #region Methods
        private void ShowWait()
        {
            WaitGrid.Visibility = Visibility.Visible;
            WaitAnimation.IsActive = true;
        }
        private void HideWait()
        {
            WaitAnimation.IsActive = false;
            WaitGrid.Visibility = Visibility.Collapsed;
        }

        private void GetColumnMeasures()
        {
            ColWidths = new List<double>();

            string txt = Properties.Settings.Default.AppointmentListDG;
            string[] cols = txt.Split('|');
            foreach (var item in cols)
            {
                string[] colPars = item.Split(new char[] { ':', ':' });
                int index = int.Parse(colPars[0]);
                double width = double.Parse(colPars[2]);

                if (index >= DG1.Columns.Count)
                    break;
                DG1.Columns[index].Width = width;
                ColWidths.Add(width);
                //Console.WriteLine(colPars);
            }
        }
        private void SetColumnMeasures()
        {
            string txt = "";
            foreach (var item in DG1.Columns)
            {
                txt += string.Format("|{0}::{1:F2}", item.DisplayIndex, item.ActualWidth);
            }
            Properties.Settings.Default.AppointmentListDG = txt.Substring(1);
            Properties.Settings.Default.Save();
        }

        private void GetDataAsync()
        {
            ShowWait();
            this.DataContext = null;            

            BackgroundWorker GetDataBW = new BackgroundWorker();
            GetDataBW.DoWork += GetDataBW_DoWork;
            GetDataBW.RunWorkerCompleted += GetDataBW_RunWorkerCompleted;
            GetDataBW.RunWorkerAsync();
        }
        private void GetDataBW_DoWork(object sender, DoWorkEventArgs e)
        {
            List<DAL.DataModel.Customer> customers = Globals.Db.GetCustomer().ToList();
            customers.Insert(0, new DAL.DataModel.Customer() { Id = 0, Name = "" });
            viewModel.CustomerList = customers.ToArray();

            List<DAL.DataModel.Doctor> doctors = Globals.Db.GetDoctor().ToList();
            doctors.Insert(0, new DAL.DataModel.Doctor() { Id = 0, Name = "" });
            viewModel.DoctorList = doctors.ToArray();

            List<DAL.DataModel.AppointmentType> appointmentTypes = Globals.Db.GetAppointmentType().ToList();
            appointmentTypes.Insert(0, new DAL.DataModel.AppointmentType() { Id = 0, Name = "" });
            viewModel.AppointmentTypeList = appointmentTypes.ToArray();

            List<DAL.DataModel.User> users = Globals.Db.GetUser().ToList();
            users.Insert(0, new DAL.DataModel.User() { Id = 0, Logon = "" });
            viewModel.UserList = users.ToArray();
            
            DAL.DataModel.Appointment[] appointment = Globals.Db.GetAppointment(viewModel.StartDate, viewModel.EndDate);

            // Filter by id
            if (!string.IsNullOrEmpty(viewModel.AppointmentId ))
            {
                var temp = appointment.Where(m => m.Id.ToString() == viewModel.AppointmentId).ToArray();
                appointment = temp;
            }

            // Filter by appointment number
            if (!string.IsNullOrEmpty(viewModel.AppointmentNumber))
            {
                var temp = appointment.Where(m => m.Number.ToString() == viewModel.AppointmentNumber).ToArray();
                appointment = temp;
            }
            

            // Filter by customer
            if (viewModel.SelectedCustomer!= null  && viewModel.SelectedCustomer.Id>0)
                appointment = appointment.Where(m => m.Id_Customer == viewModel.SelectedCustomer.Id).ToArray();

            // Filter by doctor
            if (viewModel.SelectedDoctor != null && viewModel.SelectedDoctor.Id > 0)
                appointment = appointment.Where(m => m.Id_Doctor == viewModel.SelectedDoctor.Id).ToArray();

            // Filter by appointment type
            if (viewModel.SelectedAppointmentType != null && viewModel.SelectedAppointmentType.Id > 0)
                appointment = appointment.Where(m => m.Id_AppointmentType == viewModel.SelectedAppointmentType.Id).ToArray();

            List<AppointmentVM> alist = (from a in appointment
                                         select new AppointmentVM()
                                         {
                                             AppointmentDate = a.AppointmentDate,
                                             AppointmentType = appointmentTypes.Where(m=>m.Id == a.Id_AppointmentType).FirstOrDefault()?.Name,
                                             //ClientDiscount = a.ClientDiscount,
                                             Coin = a.Coin,
                                             //ComercialDiscountVal = a.ComercialDiscountVal,
                                             Comments = a.Comments,
                                             CreationDate = a.CreationDate,
                                             CustomerName = customers.Where(m=>m.Id == a.Id_Customer).FirstOrDefault()?.Name,
                                             Description = a.Description,
                                             DoctorName = doctors.Where(m => m.Id == a.Id_Doctor).FirstOrDefault()?.Name,
                                             Exchange = a.Exchange,
                                             FinanceDiscount = a.FinanceDiscount,
                                             Id = a.Id,
                                             Id_AppointmentType = a.Id_AppointmentType,
                                             Id_Customer = a.Id_Customer,
                                             Id_Doctor = a.Id_Doctor,
                                             Id_User = a.Id_User,
                                             Number = a.Number,
                                             PaymentType = a.PaymentType,
                                             Report = a.Report,
                                             TotalProducts = a.TotalProducts,
                                             UserName = users.Where(m => m.Id == a.Id_User).FirstOrDefault()?.Name,
                                             VATValue = a.VATValue

                                         }).ToList();

            viewModel.AppointmentList = new ObservableCollection<AppointmentVM>(alist);


            //if (current.ProductId > 0)
            //{
            //    jobs = jobs.Where(m => m.Id_Product == current.ProductId).ToArray();
            //}
            //if (current.ProductCategoryId > 0)
            //{
            //    long[] CatProdIds = Globals.XClient.GetProductByCategory(Globals.ActiveCompany.Company.CompanyCode, current.ProductCategoryId).Select(m => m.Id).ToArray();
            //    jobs = (from j in jobs
            //            where CatProdIds.Contains(j.Id_Product.Value)
            //            select j).ToArray();
            //}
            //if (current.IsOffer)
            //    jobs = jobs.Where(m => m.Option1 == true).ToArray();
            //if (current.IsReturn)
            //    jobs = jobs.Where(m => m.Option2 == true).ToArray();
            //if (current.IsPendingReturn)
            //    jobs = jobs.Where(m => m.Option3 == true).ToArray();
            //if (current.IsComplaint)
            //    jobs = jobs.Where(m => m.Option4 == true).ToArray();

            //JobDetailVM[] jlist = (from j in jobs
            //                       select new JobDetailVM()
            //                       {
            //                           BeginDate = j.BeginDate,
            //                           Comments = j.Notes,
            //                           ClientId = j.Id_Entity,
            //                           CreationDate = j.CreationDate,
            //                           CurrentState = j.CurrentState,
            //                           Description = j.Description,
            //                           DisplayName = j.Description + (j.ProductLength > 0 && j.ProductLength > 0 ? string.Format("({0:F0}x{1:F0})", j.ProductWidth, j.ProductLength) : ""),
            //                           FinishDate = j.FinishDate,
            //                           Id = j.Id,
            //                           IsChecked = false,
            //                           TicketReference = j.JobReference,

            //                           Option1 = j.Option1,
            //                           Option2 = j.Option2,
            //                           Option3 = j.Option3,
            //                           Option4 = j.Option4,
            //                           Option5 = j.Option5,
            //                           Option6 = j.Option6,

            //                           ProductId = j.Id_Product,
            //                           ProductUnitType = (j.ProductLength > 0 && j.ProductWidth > 0 && j.ProductHeight == 0) ? 1 : j.ProductUnitType,
            //                           ProductWidthCm = j.ProductWidth,
            //                           ProductLengthCm = j.ProductLength,
            //                           ProductHeightCm = j.ProductHeight,

            //                           TotalValue = j.TotalValue,
            //                           UserId = j.Id_User,
            //                           ClientName = current.Entities.Where(m => m.Id == j.Id_Entity).FirstOrDefault()?.Name,
            //                           ClientIntegrationref = current.Entities.Where(m => m.Id == j.Id_Entity).FirstOrDefault()?.IntegrationRef,
            //                           ProductName = current.Products.Where(m => m.Id == j.Id_Product).FirstOrDefault()?.Name,
            //                           UserName = users.Where(m => m.Id == j.Id_User).FirstOrDefault()?.Name,
            //                           ProductIntegrationref = current.Products.Where(m => m.Id == j.Id_Product).FirstOrDefault()?.IntegrationRef,

            //                           EntityRouteId = j.Id_EntityRoute,
            //                           RouteId = entityRoute.Where(m => m.Id == j.Id_EntityRoute).FirstOrDefault()?.Id_Route,
            //                           RouteName = entityRoute.Where(m => m.Id == j.Id_EntityRoute).FirstOrDefault()?.Name,
            //                           //TicketId = 

            //                           ClientRef = j.ClientRef

            //                       }).ToArray();

            //current.JobList = new ObservableCollection<JobDetailVM>(jlist);
        }
        private void GetDataBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
                        
            this.DataContext = viewModel;

            try { GetColumnMeasures(); }
            catch
            {
                SetColumnMeasures();
                try { GetColumnMeasures(); }
                catch (Exception ex01)
                {
                    Globals.LogError(ex01);
                }
            }
        }

        #endregion

        public class LocalVM:INotifyPropertyChanged
        {
            DateTime startDate;
            DateTime endDate;

            public event PropertyChangedEventHandler PropertyChanged;

            public string AppointmentId { get; set; }
            public string AppointmentNumber { get; set; }
            
            public DateTime StartDate { get { return startDate; } set { startDate = value; OnPropertyChanged(nameof(StartDate)); } }
            public DateTime EndDate { get { return endDate; } set { endDate = value; OnPropertyChanged(nameof(EndDate)); } }

            public DAL.DataModel.Customer[] CustomerList { get; set; }
            public DAL.DataModel.Customer  SelectedCustomer { get; set; }

            public DAL.DataModel.Doctor[] DoctorList { get; set; }
            public DAL.DataModel.Doctor SelectedDoctor { get; set; }

            public DAL.DataModel.AppointmentType[] AppointmentTypeList { get; set; }
            public DAL.DataModel.AppointmentType SelectedAppointmentType { get; set; }

            public DAL.DataModel.User[] UserList { get; set; }
            public DAL.DataModel.User SelectedUser { get; set; }

            public ObservableCollection<AppointmentVM> AppointmentList { get; set; }
            public int ListCount { get { return AppointmentList.Count(); } }


            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }

        
    }
}
