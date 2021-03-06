﻿using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using NBsoft.Appointment.DAL.DataModel;
using NBsoft.Appointment.WPF.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NBsoft.Appointment.WPF.Pages
{
    /// <summary>
    /// Interaction logic for CustomersPage.xaml
    /// </summary>
    public partial class CustomersPage : UserControl,IContent
    {
        Customer[] itemList;
        CustomerVM currentItem;
        string originalValues;
                
        public CustomersPage()
        {
            InitializeComponent();
            DetailTab.SelectedSourceChanged += DetailTab_SelectedSourceChanged;
            ActivePage = this;
        }

        internal static CustomersPage ActivePage { get; private set; }
        public CustomerVM CurrentItem { get { return currentItem; } }

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
        public void SetStatus(string text, params object[] parameters)
        {
            string ftext = string.Format(text, parameters);
            Globals.Log(this.GetType().Name, ftext, Enums.LogType.Backoffice);
            TxtStatus.Text = ftext;
        }

        private void ClearData()
        {
            TxtStatus.Text = "";
            itemList = null;
            currentItem = null;
            DetailTab.SelectedSource = null;
            this.DetailLinks.Clear();
        }
        private void PopList(Customer[] cList, long selectedId)
        {
            DetailTab.SelectedSource = null;

            Customer[] customerList = cList.OrderBy(m => m.Name).ToArray();
            this.DetailLinks.Clear();
            Customer active;
            if (selectedId == 0)
                active = customerList.FirstOrDefault();
            else
            {
                active = customerList.Where(m => m.Id == selectedId).FirstOrDefault();
                if (active == null)
                    active = customerList.FirstOrDefault();
            }
            if (active == null)
            {
                Add();
            }
            else
            {
                Uri currentSource = null;
                foreach (var item in customerList)
                {
                    Uri source = new Uri(string.Format("/Content/CustomerDetails.xaml#{0}", item.Id), UriKind.Relative);
                    this.DetailLinks.Add(new FirstFloor.ModernUI.Presentation.Link() { DisplayName = " " + item.Name + " ", Source = source });
                    if (item.Id == active.Id)
                        currentSource = source;
                }
                DetailTab.SelectedSource = currentSource;
            }
            SetStatus("{0} {1}", Globals.DicMan.Get("app.customer.listcount"), cList.Length);
        }
        private void PopDetails(long id)
        {
            if (id == 0)
            {
                currentItem = new CustomerVM();
            }
            else
            {
                Customer item = itemList.Where(m => m.Id == id).FirstOrDefault();
                if (item != null)
                {
                    currentItem = CustomerVM.FromDBO(item);
                }
            }
            originalValues = currentItem.GetStringId();
        }

        private void LoadDataAsync()
        {
            ShowWait();

            BackgroundWorker LoadDataBW = new BackgroundWorker();
            LoadDataBW.DoWork += LoadDataBW_DoWork;
            LoadDataBW.RunWorkerCompleted += LoadDataBW_RunWorkerCompleted;
            LoadDataBW.RunWorkerAsync();
        }
        private void LoadDataBW_DoWork(object sender, DoWorkEventArgs e)
        {
            itemList = Globals.Db.GetCustomer();
        }
        private void LoadDataBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            PopList(itemList, 0);
        }

        private void SaveAsync(CustomerVM item)
        {
            ShowWait();

            BackgroundWorker SaveBW = new BackgroundWorker();
            SaveBW.DoWork += SaveBW_DoWork;
            SaveBW.RunWorkerCompleted += SaveBW_RunWorkerCompleted;
            SaveBW.RunWorkerAsync(item);
        }
        private void SaveBW_DoWork(object sender, DoWorkEventArgs e)
        {
            CustomerVM item = e.Argument as CustomerVM;
            Customer dbo = new Customer()
            {
                Id = item.Id,
                CreationDate = item.CreationDate,
                Name = item.Name,
                BirthDate = item.BirthDate,
                Address = item.Address,
                City = item.City,
                Contact = item.Contact,
                Country = item.Country,
                EMail = item.EMail,
                Fax = item.Fax,
                IBAN = item.IBAN,
                IntegrationDate = item.IntegrationDate,
                IntegrationRef = item.IntegrationRef,
                MobilePhone = item.MobilePhone,
                PostalCode = item.PostalCode,
                TaxIdNumber = item.TaxIdNumber,
                Telephone = item.Telephone,
                URL = item.URL,
                DrivingLicense = item.DrivingLicense,
                DrivingLicenseDate = item.DrivingLicenseDate,
                DrivingLicenseType = item.DrivingLicenseType,
                Comments = item.Comments,
                NextAppointment = item.NextAppointment
            };

            long[] ids = Globals.Db.SetCustomer(new Customer[] { dbo });
            if (ids == null || ids.Length != 1)
                throw new ApplicationException(Globals.DicMan.Get("app.database.updatefailed"));

            itemList = Globals.Db.GetCustomer();
            e.Result = ids[0];
        }
        private void SaveBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                SetStatus("[{0}] > {1}", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.customer.save.failed"));
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            
            SetStatus("[{0}] > {1} '{2}'", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.customer.save.ok"), currentItem.Name);
            long SelectedId = (long)e.Result;
            PopList(itemList, SelectedId);
        }

        private void DeleteAsync(Customer item)
        {
            ShowWait();

            BackgroundWorker DeleteBW = new BackgroundWorker();
            DeleteBW.DoWork += DeleteBW_DoWork;
            DeleteBW.RunWorkerCompleted += DeleteBW_RunWorkerCompleted;
            DeleteBW.RunWorkerAsync(item);
        }
        private void DeleteBW_DoWork(object sender, DoWorkEventArgs e)
        {
            Customer r = e.Argument as Customer;
            Globals.Db.DeleteCustomer(new long[] { r.Id });
            itemList = Globals.Db.GetCustomer();
        }
        private void DeleteBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                SetStatus("[{0}] > {1}", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.customer.delete.failed"));
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            SetStatus("[{0}] > {1} '{2}'", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.customer.delete.ok"), currentItem.Name);
            PopList(itemList, 0);
        }

        private void Add()
        {
            FirstFloor.ModernUI.Presentation.Link link = (from l in this.DetailLinks
                                                          where l.DisplayName == "<*>"
                                                          select l).FirstOrDefault();

            if (link == null)
            {
                Uri source = new Uri("/Content/CustomerDetails.xaml#0", UriKind.Relative);
                this.DetailLinks.Insert(0, new FirstFloor.ModernUI.Presentation.Link() { DisplayName = "<*>", Source = source });
                DetailTab.SelectedSource = source;
            }
            else
                DetailTab.SelectedSource = link.Source;
        }
        private void Save()
        {
            if (currentItem == null)
                return;

            string currentValues = currentItem.GetStringId();
            if (originalValues == currentValues)// No chanegs
            {
                SetStatus(Globals.DicMan.Get("app.nochanges"));
                return;
            }

            var TaxIdExists = (from c in itemList
                               where c.Id != currentItem.Id && c.TaxIdNumber == currentItem.TaxIdNumber
                               select c).FirstOrDefault();
            if (TaxIdExists != null)
            {
                string msg = string.Format("{0}:\n\t[{1}]", Globals.DicMan.Get("app.customer.save.taxidalreadyexists"), TaxIdExists.Name);
                ModernDialog.ShowMessage(msg, Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }

            SaveAsync(currentItem);
        }
        private void Delete()
        {
            Customer selected = (from r in itemList
                               where r.Id == currentItem.Id
                               select r).FirstOrDefault();
            if (selected != null)
            {
                MessageBoxResult res = ModernDialog.ShowMessage(Globals.DicMan.Get("app.customer.delete.confirm"), Globals.AppName, MessageBoxButton.YesNo, Globals.MainWnd);
                if (res == MessageBoxResult.Yes)
                    DeleteAsync(selected);
            }
        }
        private void FilterRecords(string searchstring)
        {
            Customer[] filtered;
            if (searchstring == "")
                filtered = itemList;
            else
                try
                {
                    filtered = (from t in itemList
                                where t.Name.ToLower().Contains(searchstring.ToLower()) ||
                                t.TaxIdNumber.ToLower().Contains(searchstring.ToLower()) ||
                                (t.MobilePhone != null && t.MobilePhone.ToLower().Contains(searchstring.ToLower())) ||
                                (t.Telephone != null && t.Telephone.ToLower().Contains(searchstring.ToLower())) ||
                                (t.Fax != null && t.Fax.ToLower().Contains(searchstring.ToLower())) ||
                                (t.EMail != null && t.EMail.ToLower().Contains(searchstring.ToLower())) ||
                                (t.URL != null && t.URL.ToLower().Contains(searchstring.ToLower())) ||
                                (t.Address != null && t.Address.ToLower().Contains(searchstring.ToLower())) ||
                                (t.PostalCode != null && t.PostalCode.ToLower().Contains(searchstring.ToLower())) ||
                                (t.City != null && t.City.ToLower().Contains(searchstring.ToLower())) ||
                                (t.Country != null && t.Country.ToLower().Contains(searchstring.ToLower())) ||
                                (t.IBAN != null && t.IBAN.ToLower().Contains(searchstring.ToLower())) ||                                
                                (t.Id.ToString().ToLower().Contains(searchstring.ToLower())) ||
                                (t.IntegrationRef != null && t.IntegrationRef.ToString().ToLower().Contains(searchstring.ToLower())) ||
                                (t.DrivingLicense != null && t.DrivingLicense.ToString().ToLower().Contains(searchstring.ToLower())) ||
                                (t.DrivingLicenseDate.ToString("yyyy-MM-dd").ToLower().Contains(searchstring.ToLower())) ||
                                (t.BirthDate.ToString("yyyy-MM-dd").ToLower().Contains(searchstring.ToLower())) ||                                
                                (t.Contact != null && t.Contact.ToLower().Contains(searchstring.ToLower()))
                                select t).ToArray();
                }
                catch (Exception ex01)
                {
                    Globals.LogError(ex01);
                    ModernDialog.ShowMessage(string.Format("{0}\n - {1}", Globals.DicMan.Get("app.error"), ex01.Message), ex01.Source, MessageBoxButton.OK, Globals.MainWnd);
                    filtered = itemList;
                }
            PopList(filtered, 0);


        }


        #endregion

        #region Event Handlers

        private void DetailTab_SelectedSourceChanged(object sender, SourceEventArgs e)
        {
            if (e.Source == null)
                return;
            string sId = e.Source.ToString().Split('#')[1];
            long Id = long.Parse(sId);
            PopDetails(Id);
        }

        public void OnFragmentNavigation(FragmentNavigationEventArgs e) { }
        public void OnNavigatedFrom(NavigationEventArgs e) { }
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationType != NavigationType.New)
                Console.WriteLine(e.NavigationType);

            ClearData();
            LoadDataAsync();
        }
        public void OnNavigatingFrom(NavigatingCancelEventArgs e) { }

        private void Taskbar_Add(object sender, EventArgs e) => Add();
        private void Taskbar_Delete(object sender, EventArgs e) => Delete();
        private void Taskbar_Save(object sender, EventArgs e) => Save();
        private void Taskbar_Search(object sender, Common.TextEventArgs e) => FilterRecords(e.Text);

        #endregion
    }
}
