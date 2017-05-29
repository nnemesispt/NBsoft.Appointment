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
    /// Interaction logic for AppointmentTypePage.xaml
    /// </summary>
    public partial class AppointmentTypePage : UserControl, IContent
    {
        AppointmentType[] itemList;
        AppointmentTypeVM currentItem;
        string originalValues;

        public AppointmentTypePage()
        {
            InitializeComponent();
            DetailTab.SelectedSourceChanged += DetailTab_SelectedSourceChanged;
            ActivePage = this;
        }



        internal static AppointmentTypePage ActivePage { get; private set; }
        public AppointmentTypeVM CurrentItem { get { return currentItem; } }

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
        private void PopList(AppointmentType[] cList, long selectedId)
        {
            DetailTab.SelectedSource = null;

            AppointmentType[] itemList = cList.OrderBy(m => m.Name).ToArray();
            this.DetailLinks.Clear();
            AppointmentType active;
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
                    Uri source = new Uri(string.Format("/Content/AppointmentTypeDetails.xaml#{0}", item.Id), UriKind.Relative);
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
                currentItem = new AppointmentTypeVM();
            }
            else
            {
                AppointmentType item = itemList.Where(m => m.Id == id).FirstOrDefault();
                if (item != null)
                {
                    currentItem = AppointmentTypeVM.FromDBO(item);
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
            itemList = Globals.Db.GetAppointmentType();
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

        private void SaveAsync(AppointmentTypeVM item)
        {
            ShowWait();

            BackgroundWorker SaveBW = new BackgroundWorker();
            SaveBW.DoWork += SaveBW_DoWork;
            SaveBW.RunWorkerCompleted += SaveBW_RunWorkerCompleted;
            SaveBW.RunWorkerAsync(item);
        }
        private void SaveBW_DoWork(object sender, DoWorkEventArgs e)
        {
            AppointmentTypeVM item = e.Argument as AppointmentTypeVM;
            AppointmentType dbo = new AppointmentType()
            {
                Id = item.Id,
                CreationDate = item.CreationDate,
                Name = item.Name                
            };

            long[] ids = Globals.Db.SetAppointmentType(new AppointmentType[] { dbo });
            if (ids == null || ids.Length != 1)
                throw new ApplicationException(Globals.DicMan.Get("app.database.updatefailed"));

            itemList = Globals.Db.GetAppointmentType();
            e.Result = ids[0];
        }
        private void SaveBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                SetStatus("[{0}] > {1}", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.appointmenttype.save.failed"));
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            SetStatus("[{0}] > {1} '{2}'", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.appointmenttype.save.ok"), currentItem.Name);
            long SelectedId = (long)e.Result;
            PopList(itemList, SelectedId);
        }

        private void DeleteAsync(AppointmentType item)
        {
            ShowWait();

            BackgroundWorker DeleteBW = new BackgroundWorker();
            DeleteBW.DoWork += DeleteBW_DoWork;
            DeleteBW.RunWorkerCompleted += DeleteBW_RunWorkerCompleted;
            DeleteBW.RunWorkerAsync(item);
        }
        private void DeleteBW_DoWork(object sender, DoWorkEventArgs e)
        {
            AppointmentType r = e.Argument as AppointmentType;
            Globals.Db.DeleteAppointmentType(new long[] { r.Id });
            itemList = Globals.Db.GetAppointmentType();
        }
        private void DeleteBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                SetStatus("[{0}] > {1}", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.appointmenttype.delete.failed"));
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            SetStatus("[{0}] > {1} '{2}'", Globals.LoggedUser.Logon, Globals.DicMan.Get("app.appointmenttype.delete.ok"), currentItem.Name);
            PopList(itemList, 0);
        }

        private void Add()
        {
            FirstFloor.ModernUI.Presentation.Link link = (from l in this.DetailLinks
                                                          where l.DisplayName == "<*>"
                                                          select l).FirstOrDefault();

            if (link == null)
            {
                Uri source = new Uri("/Content/AppointmentTypeDetails.xaml#0", UriKind.Relative);
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
            
            SaveAsync(currentItem);
        }
        private void Delete()
        {
            AppointmentType selected = (from r in itemList
                               where r.Id == currentItem.Id
                               select r).FirstOrDefault();
            if (selected != null)
            {
                MessageBoxResult res = ModernDialog.ShowMessage(Globals.DicMan.Get("app.appointmenttype.delete.confirm"), Globals.AppName, MessageBoxButton.YesNo, Globals.MainWnd);
                if (res == MessageBoxResult.Yes)
                    DeleteAsync(selected);
            }
        }
        private void FilterRecords(string searchstring)
        {
            AppointmentType[] filtered;
            if (searchstring == "")
                filtered = itemList;
            else
                try
                {
                    filtered = (from t in itemList
                                where t.Name.ToLower().Contains(searchstring.ToLower()) ||
                                (t.Id.ToString().ToLower().Contains(searchstring.ToLower()))
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

        private void DetailTab_SelectedSourceChanged(object sender, FirstFloor.ModernUI.Windows.Controls.SourceEventArgs e)
        {
            if (e.Source == null)
                return;
            string sId = e.Source.ToString().Split('#')[1];
            long Id = long.Parse(sId);
            PopDetails(Id);
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            throw new NotImplementedException();
        }
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationType != NavigationType.New)
                Console.WriteLine(e.NavigationType);

            ClearData();
            LoadDataAsync();
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Taskbar_Add(object sender, EventArgs e)
        {
            if (Globals.License == null)
            {
                ModernDialog.ShowMessage(Globals.DicMan.Get("app.invalidlicense"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            Add();
        }

        private void Taskbar_Delete(object sender, EventArgs e)
        {
            if (Globals.License == null)
            {
                ModernDialog.ShowMessage(Globals.DicMan.Get("app.invalidlicense"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            Delete();
        }

        private void Taskbar_Save(object sender, EventArgs e)
        {
            if (Globals.License == null)
            {
                ModernDialog.ShowMessage(Globals.DicMan.Get("app.invalidlicense"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                return;
            }
            Save();
        }

        private void Taskbar_Search(object sender, Common.TextEventArgs e)
        {
            FilterRecords(e.Text);
        }

        #endregion
    }
}
