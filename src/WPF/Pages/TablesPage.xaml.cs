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

namespace NBsoft.Appointment.WPF.Pages
{
    /// <summary>
    /// Interaction logic for TablesPage.xaml
    /// </summary>
    public partial class TablesPage : UserControl, IContent
    {
        public TablesPage()
        {
            InitializeComponent();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {            
        }
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            PnlLinks.Children.Clear();
            PnlLinks.Children.Add(new FirstFloor.ModernUI.Windows.Controls.BBCodeBlock() { BBCode= string.Format("[url=/Pages/CustomersPage.xaml|_self]{0}[/url]", Globals.DicMan.Get("app.tables.customers")) });
            PnlLinks.Children.Add(new FirstFloor.ModernUI.Windows.Controls.BBCodeBlock() { BBCode = string.Format("[url=/Pages/DoctorsPage.xaml|_self]{0}[/url]", Globals.DicMan.Get("app.tables.doctors")) });
            PnlLinks.Children.Add(new FirstFloor.ModernUI.Windows.Controls.BBCodeBlock() { BBCode = string.Format("[url=/Pages/AppointmentTypePage.xaml|_self]{0}[/url]", Globals.DicMan.Get("app.tables.appointmenttype")) });

        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }
    }
}
