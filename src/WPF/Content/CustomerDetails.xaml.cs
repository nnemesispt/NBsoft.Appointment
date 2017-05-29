using FirstFloor.ModernUI.Windows;
using NBsoft.Appointment.WPF.ViewModels;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NBsoft.Appointment.WPF.Content
{
    /// <summary>
    /// Interaction logic for CustomerDetails.xaml
    /// </summary>
    public partial class CustomerDetails : UserControl,IContent
    {
        LocalVM viewModel;
        public CustomerDetails()
        {
            InitializeComponent();
            viewModel = new LocalVM();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            DataContext = null;
            viewModel.Customer = Pages.CustomersPage.ActivePage.CurrentItem;
            if (viewModel.Customer.NextAppointment.HasValue )
            {
                if(viewModel.Customer.NextAppointment.Value> DateTime.Today)
                    viewModel.NextAppointmentBrush = Brushes.Green;
                else
                    viewModel.NextAppointmentBrush = Brushes.Red;
            }
            else
                viewModel.NextAppointmentBrush = Brushes.Orange;

                DAL.DataModel.Appointment[] appointments = Globals.Db.GetAppointmentByCustomer(viewModel.Customer.Id);
            if (appointments != null)
                viewModel.AppointmentList = new ObservableCollection<AppointmentVM>(AppointmentVM.FromDBO(appointments));
            else
                viewModel.AppointmentList = new ObservableCollection<AppointmentVM>();

            DataContext = viewModel;
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            DataContext = null;
        }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {   
        }


        private void BtnClearPreviewDate_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Customer.NextAppointment = null;
        }

        public class LocalVM: INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            Brush nextAppointmentBrush;

            public CustomerVM Customer { get; set; }
            public ObservableCollection<AppointmentVM> AppointmentList { get; set; }
            public Brush NextAppointmentBrush { get { return nextAppointmentBrush; } set { nextAppointmentBrush = value; OnPropertyChanged(nameof(NextAppointmentBrush)); } }


            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

     
    }
}
