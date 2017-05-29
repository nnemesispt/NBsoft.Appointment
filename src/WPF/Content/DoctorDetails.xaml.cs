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

namespace NBsoft.Appointment.WPF.Content
{
    /// <summary>
    /// Interaction logic for DoctorDetails.xaml
    /// </summary>
    public partial class DoctorDetails : UserControl,IContent
    {
        LocalVM viewModel;
        public DoctorDetails()
        {
            InitializeComponent();
            viewModel = new LocalVM();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            DataContext = null;
            viewModel.Doctor = Pages.DoctorsPage.ActivePage.CurrentItem;
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

        public class LocalVM
        {
            public DoctorVM Doctor { get; set; }
        }
    }
}
