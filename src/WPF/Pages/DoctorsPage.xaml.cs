using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using NBsoft.Appointment.DAL.DataModel;
using NBsoft.Appointment.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace NBsoft.Appointment.WPF.Pages
{
    /// <summary>
    /// Interaction logic for DoctorsPage.xaml
    /// </summary>
    public partial class DoctorsPage : UserControl, IContent
    {
        Doctor[] itemList;
        DoctorVM currentItem;
        string originalValues;

        public DoctorsPage()
        {
            InitializeComponent();
            DetailTab.SelectedSourceChanged += DetailTab_SelectedSourceChanged;
            ActivePage = this;
        }

        
        internal static DoctorsPage ActivePage { get; private set; }
        public DoctorVM CurrentItem { get { return currentItem; } }

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
        private void PopList(Doctor[] cList, long selectedId)
        {
            DetailTab.SelectedSource = null;

            Doctor[] itemList = cList.OrderBy(m => m.Name).ToArray();
            this.DetailLinks.Clear();
            Doctor active;
            if (selectedId == 0)
                active = itemList.FirstOrDefault();
            else
            {
                active = itemList.Where(m => m.Id == selectedId).FirstOrDefault();
                if (active == null)
                    active = itemList.FirstOrDefault();
            }
            if (active == null)
            {
                Add();
            }
            else
            {
                Uri currentSource = null;
                foreach (var item in itemList)
                {
                    Uri source = new Uri(string.Format("/Content/DoctorDetails.xaml#{0}", item.Id), UriKind.Relative);
                    this.DetailLinks.Add(new FirstFloor.ModernUI.Presentation.Link() { DisplayName = " " + item.Name + " ", Source = source });
                    if (item.Id == active.Id)
                        currentSource = source;
                }
                DetailTab.SelectedSource = currentSource;
            }
        }
        private void PopDetails(long id)
        {
            if (id == 0)
            {
                currentItem = new DoctorVM();
            }
            else
            {
                Doctor item = itemList.Where(m => m.Id == id).FirstOrDefault();
                if (item != null)
                {
                    currentItem = DoctorVM.FromDBO(item);
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
            itemList = Globals.Db.GetDoctor();
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

        private void SaveAsync(DoctorVM item)
        {
            ShowWait();

            BackgroundWorker SaveBW = new BackgroundWorker();
            SaveBW.DoWork += SaveBW_DoWork;
            SaveBW.RunWorkerCompleted += SaveBW_RunWorkerCompleted;
            SaveBW.RunWorkerAsync(item);
        }
        private void SaveBW_DoWork(object sender, DoWorkEventArgs e)
        {
            DoctorVM item = e.Argument as DoctorVM;
            Doctor dbo = new Doctor()
            {
                Id = item.Id,
                CreationDate = item.CreationDate,
                Name = item.Name,
                Address = item.Address,
                City = item.City,
                Contact = item.Contact,
                Country = item.Country,
                EMail = item.EMail,
                Fax = item.Fax,
                IBAN = item.IBAN,                
                MobilePhone = item.MobilePhone,
                PostalCode = item.PostalCode,
                TaxIdNumber = item.TaxIdNumber,
                Telephone = item.Telephone,
                URL = item.URL,
                Comments = item.Comments
            };

            long[] ids = Globals.Db.SetDoctor(new Doctor[] { dbo });
            if (ids == null || ids.Length != 1)
                throw new ApplicationException(Globals.DicMan.Get("app.database.updatefailed"));

            itemList = Globals.Db.GetDoctor();
            e.Result = ids[0];
        }
        private void SaveBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                SetStatus("[{0}] > {1}", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.doctor.save.failed"));
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            SetStatus("[{0}] > {1} '{2}'", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.doctor.save.ok"), currentItem.Name);
            long SelectedId = (long)e.Result;
            PopList(itemList, SelectedId);
        }

        private void DeleteAsync(Doctor item)
        {
            ShowWait();

            BackgroundWorker DeleteBW = new BackgroundWorker();
            DeleteBW.DoWork += DeleteBW_DoWork;
            DeleteBW.RunWorkerCompleted += DeleteBW_RunWorkerCompleted;
            DeleteBW.RunWorkerAsync(item);
        }
        private void DeleteBW_DoWork(object sender, DoWorkEventArgs e)
        {
            Doctor r = e.Argument as Doctor;
            Globals.Db.DeleteDoctor(new long[] { r.Id });
            itemList = Globals.Db.GetDoctor();
        }
        private void DeleteBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                SetStatus("[{0}] > {1}", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.doctor.delete.failed"));
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            SetStatus("[{0}] > {1} '{2}'", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.doctor.delete.ok"), currentItem.Name);
            PopList(itemList, 0);
        }

        private void Add()
        {
            FirstFloor.ModernUI.Presentation.Link link = (from l in this.DetailLinks
                                                          where l.DisplayName == "<*>"
                                                          select l).FirstOrDefault();

            if (link == null)
            {
                Uri source = new Uri("/Content/DoctorDetails.xaml#0", UriKind.Relative);
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
                string msg = string.Format("{0}:\n\t[{1}]", Globals.DicMan.Get("app.doctor.save.taxidalreadyexists"), TaxIdExists.Name);
                ModernDialog.ShowMessage(msg, Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }

            SaveAsync(currentItem);
        }
        private void Delete()
        {
            Doctor selected = (from r in itemList
                                 where r.Id == currentItem.Id
                                 select r).FirstOrDefault();
            if (selected != null)
            {
                MessageBoxResult res = ModernDialog.ShowMessage(Globals.DicMan.Get("app.doctor.delete.confirm"), Globals.AppName, MessageBoxButton.YesNo, Globals.MainWnd);
                if (res == MessageBoxResult.Yes)
                    DeleteAsync(selected);
            }
        }
        private void FilterRecords(string searchstring)
        {
            Doctor[] filtered;
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
                                (t.Comments.ToLower().Contains(searchstring.ToLower())) ||
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

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e) { }
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e) { }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationType != NavigationType.New)
                Console.WriteLine(e.NavigationType);

            ClearData();
            LoadDataAsync();
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e) { }

        private void Taskbar_Add(object sender, EventArgs e) => Add();
        private void Taskbar_Delete(object sender, EventArgs e) => Delete();
        private void Taskbar_Save(object sender, EventArgs e) => Save();
        private void Taskbar_Search(object sender, Common.TextEventArgs e) => FilterRecords(e.Text);

        #endregion
    }
}
