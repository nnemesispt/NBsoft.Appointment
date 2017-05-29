using FirstFloor.ModernUI.Presentation;
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
    /// Interaction logic for SettingsAppearance.xaml
    /// </summary>
    public partial class SettingsAppearance : UserControl
    {
        public SettingsAppearance()
        {
            InitializeComponent();

            this.DataContext = new SettingsAppearanceViewModel(AppearanceManager.Current.FontSize, AppearanceManager.Current.ThemeSource, AppearanceManager.Current.AccentColor);
            CmbTheme.SelectionChanged += CmbTheme_SelectionChanged;
            LbxAccentColor.SelectionChanged += LbxAccentColor_SelectionChanged;
        }

        private void LbxAccentColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Color color = ((SettingsAppearanceViewModel)this.DataContext).SelectedAccentColor;
            int ColorS = Globals.GetIntFromColor(color);
            if (Globals.LoggedUser.Accentcolor == ColorS)
                return;

            Globals.LoggedUser.Accentcolor = ColorS;
            Globals.Log("MainWindow", string.Format("Saving AccentColor: {0}", ColorS));

            try { Globals.Db.SetUser(new DAL.DataModel.User[] { Globals.LoggedUser }); }
            catch (Exception ex01)
            { Globals.LogError(ex01); }

            try
            {
                ResourceDictionary resources = App.Current.Resources; // If in a Window/UserControl/etc
                resources["UserAccent"] = new System.Windows.Media.SolidColorBrush(color);
            }
            catch (Exception ex01)
            { Globals.LogError(ex01); }
        }

        private void CmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var context = ((SettingsAppearanceViewModel)this.DataContext).SelectedTheme;
            string theme = string.Format("{0}|{1}", context.DisplayName, context.Source.ToString());
            if (Globals.LoggedUser.Theme == theme)
                return;
            Globals.LoggedUser.Theme = theme;
            Globals.Log("MainWindow", string.Format("Saving Theme: {0}", context.Source));

            try { Globals.Db.SetUser(new DAL.DataModel.User[] { Globals.LoggedUser }); }
            catch (Exception ex01)
            { Globals.LogError(ex01); }



        }
    }
}
