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

namespace NBsoft.Appointment.WPF.Content
{
    /// <summary>
    /// Interaction logic for SettingsLanguage.xaml
    /// </summary>
    public partial class SettingsLanguage : UserControl,IContent
    {
        List<object> Languages;

        public SettingsLanguage()
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
            LoadLanguage();
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void CmbDictionary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbDictionary.SelectedItem != null)
            {
                dynamic d = CmbDictionary.SelectedItem;
                Dictionary.Language l = d.LanguageO;
                Globals.DicMan.SetActive(l);
                if (l.CultureInfo.ToString() == Globals.LoggedUser.Language)
                    return;

                Globals.LoggedUser.Language = l.CultureInfo.ToString();
                Globals.Log("MainWindow", string.Format("Saving Dictionary: {0}", l.CultureInfo));

                try { Globals.Db.SetUser(new DAL.DataModel.User[] { Globals.LoggedUser }); }
                catch (Exception ex01)
                { Globals.LogError(ex01); }


            }
        }

        private void LoadLanguage()
        {
            Languages = new List<object>();
            object selected = null;
            foreach (var item in Globals.DicMan.AvailableLanguages)
            {
                string image = item.Image;
                ImageSource source;
                try { source = Globals.BitmapFromUri(new Uri(item.Image, UriKind.Absolute)); }
                catch { source = null; }
                string text = item.CultureInfo.NativeName;

                dynamic li = new { FlagImage = source, LanguageName = text, LanguageO = item };
                Languages.Add(li);
                if (Globals.DicMan.ActiveLanguage == item)
                    selected = li;
            }

            CmbDictionary.ItemsSource = Languages;
            CmbDictionary.SelectedItem = selected;


        }
    }
}
