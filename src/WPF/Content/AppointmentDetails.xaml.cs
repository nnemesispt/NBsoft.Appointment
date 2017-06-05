using FirstFloor.ModernUI.Windows;
using System;
using System.Collections.Generic;
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
using NBsoft.Appointment.WPF.Pages;
using System.ComponentModel;
using FirstFloor.ModernUI.Windows.Controls;
using System.Runtime.CompilerServices;

namespace NBsoft.Appointment.WPF.Content
{
    /// <summary>
    /// Interaction logic for AppointmentDetails.xaml
    /// </summary>
    public partial class AppointmentDetails : UserControl,IContent
    {
        LocalVM viewModel;

        public AppointmentDetails()
        {
            InitializeComponent();
            viewModel = new LocalVM();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            string[] vals = e.Fragment.Split('=');
            string parametername = vals[0].ToLower();
            long Id = long.Parse(vals[1]);
            switch (parametername)
            {
                case "id":
                    try
                    {
                        viewModel.Appointment = null;
                        GetDataAsync(Id);
                    }
                    catch (Exception ex01) { Globals.LogError(ex01); }
                    break;
                case "customer":
                    try
                    {                        
                        DAL.DataModel.Customer c = viewModel.CustomerList.Where(m => m.Id == Id).FirstOrDefault();
                        if (c != null)
                        {
                            viewModel.Appointment.Id_Customer = c.Id;
                            viewModel.Appointment.CustomerName = c.Name;
                        }

                    }
                    catch { throw; }
                    break;
                case "appointment":
                    try
                    {
                        DAL.DataModel.AppointmentType c = viewModel.AppointmentTypeList.Where(m => m.Id == Id).FirstOrDefault();
                        if (c != null)
                        {
                            viewModel.Appointment.Id_AppointmentType = c.Id;
                            viewModel.Appointment.AppointmentType = c.Name;
                        }
                    }
                    catch { throw; }
                    break;
                case "doctor":
                    try
                    {
                        DAL.DataModel.Doctor c = viewModel.DoctorList.Where(m => m.Id == Id).FirstOrDefault();
                        if (c != null)
                        {
                            viewModel.Appointment.Id_Doctor = c.Id;
                            viewModel.Appointment.DoctorName = c.Name;
                        }
                    }
                    catch { throw; }
                    break;
            }

        }
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            //this.DataContext = null;
        }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {            
        }

        private async void Taskbar_Save(object sender, EventArgs e)
        {
            if (Globals.License == null)
            {
                ModernDialog.ShowMessage(Globals.DicMan.Get("app.invalidlicense"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            // Removed License limits for freeware version
            //if (Globals.License.LicenseTypeC != Enums.LicenseType.Pro)
            //{
            //    // Free License only allows 10 new appointments/month
            //    if (viewModel.Appointment.Id == 0)
            //    {
            //        DateTime starDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            //        var appointments = Globals.Db.GetAppointment(starDate, DateTime.Today);
            //        if (appointments.Length >= 10)
            //        {
            //            ModernDialog.ShowMessage(Globals.DicMan.Get("app.license.limitreached"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
            //            return;
            //        }
            //    }
            //}


            ShowWait();
            long[] result = await SaveAsync();
            HideWait();

            if (result != null)
            {
                NavigationService nav = NavigationService.GetNavigationService(AppointmentsPage.ActivePage);
                if (nav != null)
                    nav.GoBack();
                //string url = "/MainWindow.xaml";
                
                //NavigationCommands.BrowseBack.Execute();

            }
        }        
        private void Taskbar_Add(object sender, EventArgs e)
        {
            if (Globals.License == null)
            {
                ModernDialog.ShowMessage(Globals.DicMan.Get("app.invalidlicense"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            this.DataContext = null;
            GetDataAsync(0);
            this.DataContext = viewModel;
        }
        private void Taskbar_Delete(object sender, EventArgs e)
        {
            if (Globals.License == null)
            {
                ModernDialog.ShowMessage(Globals.DicMan.Get("app.invalidlicense"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            bool deleteOk = Delete();
            if (deleteOk)
            {
                this.DataContext = null;                
            }
        }
                
        private void BtnSearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            AppointmentsPage.ActivePage.LastControl = new Uri("/Content/AppointmentDetails.xaml", UriKind.Relative);
            AppointmentsPage.ActivePage.TableList = viewModel.CustomerList;
            AppointmentsPage.ActivePage.TableListSelected = null;

            string url = "/Content/TableSearch.xaml";
            BBCodeBlock bbBlock = new BBCodeBlock();
            bbBlock.LinkNavigator.Navigate(new Uri(url, UriKind.Relative), this, NavigationHelper.FrameSelf);
        }
        private void BtnSearchDoctor_Click(object sender, RoutedEventArgs e)
        {
            AppointmentsPage.ActivePage.LastControl = new Uri("/Content/AppointmentDetails.xaml", UriKind.Relative);
            AppointmentsPage.ActivePage.TableList = viewModel.DoctorList;
            AppointmentsPage.ActivePage.TableListSelected = null;

            string url = "/Content/TableSearch.xaml";
            BBCodeBlock bbBlock = new BBCodeBlock();
            bbBlock.LinkNavigator.Navigate(new Uri(url, UriKind.Relative), this, NavigationHelper.FrameSelf);
        }
        private void BtnSearchAppointmentType_Click(object sender, RoutedEventArgs e)
        {
            AppointmentsPage.ActivePage.LastControl = new Uri("/Content/AppointmentDetails.xaml", UriKind.Relative);
            AppointmentsPage.ActivePage.TableList = viewModel.AppointmentTypeList;
            AppointmentsPage.ActivePage.TableListSelected = null;

            string url = "/Content/TableSearch.xaml";
            BBCodeBlock bbBlock = new BBCodeBlock();
            bbBlock.LinkNavigator.Navigate(new Uri(url, UriKind.Relative), this, NavigationHelper.FrameSelf);
        }

        private void TxtCustomerName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TxtDoctorName.Focus();
        }
        private void TxtCustomerName_LostFocus(object sender, RoutedEventArgs e)
        {
            CustomerAutoSearch(TxtCustomerName.Text);
        }

        private void TxtDoctorName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TxtAppointmentTypeName.Focus();
        }
        private void TxtDoctorName_LostFocus(object sender, RoutedEventArgs e)
        {
            DoctorAutoSearch(TxtDoctorName.Text);
        }

        private void TxtAppointmentTypeName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                DtpAppointmentDate.Focus();
        }
        private void TxtAppointmentTypeName_LostFocus(object sender, RoutedEventArgs e)
        {
            AppointmentTypeAutoSearch(TxtAppointmentTypeName.Text);
        }

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

        private void GetDataAsync(long id)
        {
            ShowWait();
            BackgroundWorker GetDataBW = new BackgroundWorker();
            GetDataBW.DoWork += GetDataBW_DoWork;
            GetDataBW.RunWorkerCompleted += GetDataBW_RunWorkerCompleted;
            GetDataBW.RunWorkerAsync(id);
        }
        private void GetDataBW_DoWork(object sender, DoWorkEventArgs e)
        {
            long Id = (long)e.Argument;            

            viewModel.Appointment = null;
            viewModel.UserList = Globals.Db.GetUser();
            viewModel.CustomerList= Globals.Db.GetCustomer();
            viewModel.DoctorList = Globals.Db.GetDoctor();
            viewModel.AppointmentTypeList = Globals.Db.GetAppointmentType();

            viewModel.PaymentTypeList = Globals.GetPaymentTypeTable();
            viewModel.CoinList = Globals.GetCoinTable();

            if (Id >0)
            {
                var dbo = Globals.Db.GetAppointmentById(Id);
                viewModel.Appointment = AppointmentVM.FromDBO(dbo);
                viewModel.Appointment.CustomerName = viewModel.CustomerList.Where(m => m.Id == viewModel.Appointment.Id_Customer).FirstOrDefault()?.Name;
                viewModel.Appointment.DoctorName = viewModel.DoctorList.Where(m => m.Id == viewModel.Appointment.Id_Doctor).FirstOrDefault()?.Name;
                viewModel.Appointment.UserName = viewModel.UserList.Where(m => m.Id == viewModel.Appointment.Id_User).FirstOrDefault()?.Name;
                viewModel.Appointment.AppointmentType = viewModel.AppointmentTypeList.Where(m => m.Id == viewModel.Appointment.Id_AppointmentType).FirstOrDefault()?.Name;
                viewModel.SelectedCoin = viewModel.CoinList.Where(m => m.Reference == viewModel.Appointment.Coin).FirstOrDefault();
            }

        }
        private void GetDataBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            this.DataContext = null;
            
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }

            if (viewModel.Appointment == null)            
                Add();
            this.DataContext = viewModel;
        }

        private void Add()
        {
            long lastnumber = Globals.Db.GetAppointmentMaxNumber();
            viewModel.SelectedCoin = viewModel.CoinList.First();
            DAL.DataModel.PaymentType ptype = viewModel.PaymentTypeList.First();
            
            viewModel.Appointment = new AppointmentVM()
            {
                Id_User = Globals.LoggedUser.Id,
                UserName = Globals.LoggedUser.Logon,
                CreationDate = DateTime.Now,
                AppointmentDate = DateTime.Today,
                Number = lastnumber + 1,
                Coin = viewModel.SelectedCoin.Reference,
                Exchange = viewModel.SelectedCoin.Exchange,
                PaymentType = ptype.Reference,
                Report = "nbsoft.appointment.appointmentfile001.rpt"
            };

            AppointmentsPage.ActivePage.SetStatus(Globals.DicMan.Get("app.appointment.new"));            
        }
        private bool Delete()
        {
            if (viewModel.Appointment == null)
            {
                AppointmentsPage.ActivePage.SetStatus(Globals.DicMan.Get("app.invalidselection"));
                return false;
            }

            //Confirm Delete
            var result = ModernDialog.ShowMessage(Globals.DicMan.Get("app.appointment.delete.confirm"), Globals.AppName, MessageBoxButton.YesNo, Globals.MainWnd);
            if (result != MessageBoxResult.Yes)
                return false;

            try
            {
                Globals.Db.DeleteAppointment(new long[] { viewModel.Appointment.Id });
                AppointmentsPage.ActivePage.SetStatus(Globals.DicMan.Get("app.appointment.delete.ok"));
                return true;
            }
            catch (Exception ex01)
            {                
                Globals.LogError(ex01);
                AppointmentsPage.ActivePage.SetStatus(Globals.DicMan.Get("app.appointment.delete.failed"));

                string msg = ex01.Message;
                if (ex01.InnerException != null)
                    msg = ex01.InnerException.Message;
                ModernDialog.ShowMessage(ex01.Message, "Error", MessageBoxButton.OK, Globals.MainWnd);
                return false;
            }
        }

        private async Task<long[]> SaveAsync()
        {
            AppointmentVM[] appointments = new AppointmentVM[] { viewModel.Appointment };
            long[] result;
            try
            {
                DAL.DataModel.Appointment[] tosave = (from a in appointments
                                                      select new DAL.DataModel.Appointment()
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
                                                          ClientDiscount = 0,
                                                          FinanceDiscount = a.FinanceDiscount,
                                                          PaymentType = a.PaymentType,
                                                          Coin = a.Coin,
                                                          Exchange = a.Exchange,
                                                          TotalProducts = a.TotalProducts,
                                                          VATValue = a.VATValue,
                                                          ComercialDiscountVal = 0,
                                                          Report = a.Report,
                                                          Comments = a.Comments
                                                      }).ToArray();


                Task<long[]> TaskSaveAppointment = Task<long[]>.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(200);
                    return Globals.Db.SetAppointment(tosave);
                });

                result = await TaskSaveAppointment;
                viewModel.Appointment.Id = result[0];

                AppointmentsPage.ActivePage.SetStatus(Globals.DicMan.Get("app.appointment.save.ok"));
            }
            catch (Exception ex01)
            {
                Globals.LogError(ex01);
                AppointmentsPage.ActivePage.SetStatus(Globals.DicMan.Get("app.appointment.save.failed"));

                string msg = ex01.Message;
                if (ex01.InnerException != null)
                    msg = ex01.InnerException.Message;
                ModernDialog.ShowMessage(ex01.Message, "Error", MessageBoxButton.OK, Globals.MainWnd);
                return null;
            }
            if (result.Length == 1)
                return result;
            else
                return null;
        }

        private void AppointmentTypeAutoSearch(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var appointmentType = (from c in viewModel.AppointmentTypeList
                                   where c.Name.ToLower().Contains(text.ToLower())
                                   select c).FirstOrDefault();
            if (appointmentType == null)
                appointmentType = (from c in viewModel.AppointmentTypeList
                                   where c.Id.ToString().Contains(text)
                                   select c).FirstOrDefault();

            if (appointmentType != null)
            {
                viewModel.Appointment.Id_AppointmentType = appointmentType.Id;
                viewModel.Appointment.AppointmentType = appointmentType.Name;
            }

        }
        private void CustomerAutoSearch(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var customer = (from c in viewModel.CustomerList
                           where c.Name.ToLower().Contains(text.ToLower())
                           select c).FirstOrDefault();
            if (customer== null)
                customer = (from c in viewModel.CustomerList
                            where c.Id.ToString().Contains(text)
                            select c).FirstOrDefault();

            if (customer != null)
            {
                viewModel.Appointment.Id_Customer = customer.Id;
                viewModel.Appointment.CustomerName = customer.Name;
            }

        }
        private void DoctorAutoSearch(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var doctor = (from c in viewModel.DoctorList
                            where c.Name.ToLower().Contains(text.ToLower())
                            select c).FirstOrDefault();
            if (doctor == null)
                doctor = (from c in viewModel.DoctorList
                            where c.Id.ToString().Contains(text)
                            select c).FirstOrDefault();

            if (doctor != null)
            {
                viewModel.Appointment.Id_Doctor= doctor.Id;
                viewModel.Appointment.DoctorName = doctor.Name;
            }

        }

        
       


        #endregion

        public class LocalVM: INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            DAL.DataModel.Coin selectedCoin;

            public DAL.DataModel.Coin SelectedCoin { get { return selectedCoin; } set { selectedCoin = value; UpdateAppointmentCoin(); OnPropertyChanged(nameof(SelectedCoin));  } }
                        
            public AppointmentVM Appointment { get; set; }
            public DAL.DataModel.User[] UserList { get; set; }
            public DAL.DataModel.Doctor[] DoctorList { get; set; }
            public DAL.DataModel.Customer[] CustomerList { get; set; }
            public DAL.DataModel.AppointmentType[] AppointmentTypeList { get; set; }

            public DAL.DataModel.PaymentType[] PaymentTypeList { get; set; }
            public DAL.DataModel.Coin[] CoinList{ get; set; }

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private void UpdateAppointmentCoin()
            {
                if (Appointment == null || SelectedCoin == null)
                    return;
                else
                {
                    if (Appointment.Coin != selectedCoin.Reference)
                    {
                        Appointment.Coin = selectedCoin.Reference;
                        Appointment.Exchange = selectedCoin.Exchange;
                    }
                }
            }
        }

        
    }
}
