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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl,IContent
    {
        public SettingsPage()
        {
            InitializeComponent();

#if !DEBUG
            MenuTab.Links.Remove(TabDebug);
#endif

            Globals.DicMan.LanguageChanged += DicMan_LanguageChanged;
        }
        private void DicMan_LanguageChanged(object sender, Dictionary.LanguageChangedEventArgs e)
        {
            ApplyLanguague();
        }

        private void ApplyLanguague()
        {
            TabAppearance.DisplayName = Globals.DicMan.Get("app.settings.appearance");          // Appearance
            TabLanguage.DisplayName = Globals.DicMan.Get("app.settings.language");              // Language

        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {   
        }
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            ApplyLanguague();
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }
    }
}
