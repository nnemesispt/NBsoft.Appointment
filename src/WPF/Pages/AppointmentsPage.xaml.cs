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
using NBsoft.Appointment.WPF.Content;
using NBsoft.Appointment.DAL.DataModel;

namespace NBsoft.Appointment.WPF.Pages
{
    /// <summary>
    /// Interaction logic for AppointmentsPage.xaml
    /// </summary>
    public partial class AppointmentsPage : UserControl, IContent
    {
        internal static AppointmentsPage ActivePage { get; private set; }
        //internal AppointmentVM CurrentAppointment { get; private set; }
        public Uri LastControl { get; internal set; }
        public Array TableList { get; internal set; }
        public TableSearch.TableItem TableListSelected { get; internal set; }

        public AppointmentsPage()
        {            
            InitializeComponent();
            ActivePage = this;
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            long appId;
            if (long.TryParse(e.Fragment, out appId))
            {
                AppointmentVM app = AppointmentVM.FromDBO(Globals.Db.GetAppointmentById(appId));
                ShowDetails(app);
            }
        }
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            DetailsTabItem.DisplayName = Globals.DicMan.Get("app.appointment.details");
            ListTabItem.DisplayName = Globals.DicMan.Get("app.appointment.list");
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {   
        }

        public void ShowDetails(AppointmentVM appointment)
        {
            MainTab.SelectedSource = null;
            long id = 0;
            if (appointment != null)
                id = appointment.Id;
            MainTab.SelectedSource = new Uri(string.Format("/Content/AppointmentDetails.xaml#id={0}", id), UriKind.Relative);
        }
        public void ShowList()
        {
            MainTab.SelectedSource = new Uri("/Content/AppointmentList.xaml", UriKind.Relative);
        }

        internal void SetSelection(AppointmentVM appointment)
        {
            long Id = 0;
            if (appointment != null)
                Id = appointment.Id;
            DetailsTabItem.Source = new Uri(string.Format("/Content/AppointmentDetails.xaml#id={0}", Id), UriKind.Relative);
        }

        public void SetStatus(string text, params object[] parameters)
        {
            string ftext = string.Format(text, parameters);
            Globals.Log(this.GetType().Name, ftext, Enums.LogType.Backoffice);
            TxtStatus.Text = ftext;
        }


        public void TableListOk(TableSearch.TableItem selected)
        {
            string fragment = "";
            if (selected.Tag is DAL.DataModel.Customer)
                fragment = string.Format("#customer={0}", selected.Id);
            else if (selected.Tag is DAL.DataModel.AppointmentType)
                fragment = string.Format("#appointment={0}", selected.Id);
            else if (selected.Tag is DAL.DataModel.Doctor)
                fragment = string.Format("#doctor={0}", selected.Id);
            else
            {
                MainTab.SelectedSource = DetailsTabItem.Source;
                return;
            }
            string newUri = LastControl.OriginalString + fragment;
            DetailsTabItem.Source = new Uri(newUri, UriKind.Relative);
            MainTab.SelectedSource = new Uri(newUri, UriKind.Relative); ;
        }
        public void TableListCancel()
        {
        }
    }
}
