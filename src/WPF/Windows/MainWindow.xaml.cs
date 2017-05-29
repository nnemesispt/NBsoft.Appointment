using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
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

namespace NBsoft.Appointment.WPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Globals.Log("MainWindow", string.Format("NBsoft.Appointment UI started by [{0}]", Globals.LoggedUser.Logon));

            // Apply Window State Size and Position
            int lastWindowState = Properties.Settings.Default.LastWindowState;
            if (lastWindowState == 1 || lastWindowState < 0 || lastWindowState > 2)
                lastWindowState = 1;

            this.Width = 1366;
            this.Height = 768;
            this.Top = 10;
            this.Left = 10;
            // Apply Last Window State
            this.WindowState = (WindowState)lastWindowState;
            if (this.WindowState == WindowState.Normal)
            {
                // If Window State is normal, apply last size and position
                this.Left = Properties.Settings.Default.LastWindowPosition.X;
                this.Top = Properties.Settings.Default.LastWindowPosition.Y;
                this.Width = Properties.Settings.Default.LastWindowSize.Width;
                this.Height = Properties.Settings.Default.LastWindowSize.Height;

                bool HasChanges = false;
                if (this.Width < 10)
                {
                    this.Width = 1366;
                    HasChanges = true;
                }
                if (this.Height < 10)
                {
                    this.Height = 768;
                    HasChanges = true;
                }
                if (this.Top < 0 || this.Top > System.Windows.SystemParameters.PrimaryScreenHeight)
                {
                    this.Top = 10;
                    HasChanges = true;
                }
                if (this.Left< 0 || this.Left> System.Windows.SystemParameters.PrimaryScreenWidth)
                {
                    this.Left = 10;
                    HasChanges = true;
                }
                if (HasChanges)
                {
                    Properties.Settings.Default.LastWindowPosition = new System.Drawing.Point((int)this.Left, (int)this.Top);
                    Properties.Settings.Default.LastWindowSize = new System.Drawing.Size((int)this.Width, (int)this.Height);
                    Properties.Settings.Default.Save();
                }
            }


            this.StateChanged += MainWindow_StateChanged;
            this.SizeChanged += MainWindow_SizeChanged;
            this.LocationChanged += MainWindow_LocationChanged;

            HeaderUser.DisplayName = Globals.LoggedUser.Name;

            ApplyLanguage();
            Globals.DicMan.LanguageChanged += DicMan_LanguageChanged;


            LoadUserSettings(); ;
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Normal)
                return;
            Properties.Settings.Default.LastWindowPosition = new System.Drawing.Point((int)this.Left, (int)this.Top);            
            Properties.Settings.Default.Save();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState != WindowState.Normal)
                return;
            Properties.Settings.Default.LastWindowSize = new System.Drawing.Size((int)this.Width, (int)this.Height);
            Properties.Settings.Default.Save();
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                Properties.Settings.Default.LastWindowState = (int)this.WindowState;
                Properties.Settings.Default.Save();
            }

        }

        private void DicMan_LanguageChanged(object sender, Dictionary.LanguageChangedEventArgs e)
        {
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            ResourceDictionary dic = Globals.DicMan.Dictionary;
            this.Resources.MergedDictionaries.Add(dic);

            HeaderSettings.DisplayName = Globals.DicMan.Get("app.mainwindow.settings");             // Settings
            HeaderHelp.DisplayName = Globals.DicMan.Get("app.mainwindow.help");                     // Help

            MnuStart.DisplayName = Globals.DicMan.Get("app.mainwindow.start");                      // Start
            MnuWelcome.DisplayName = Globals.DicMan.Get("app.mainwindow.welcome");                      // Welcome
            MnuDashboard.DisplayName = Globals.DicMan.Get("app.mainwindow.dashboard");                  // Dashboard

            MnuTables.DisplayName = Globals.DicMan.Get("app.mainwindow.tables");                    // Tables
            MnuTablesWelcome.DisplayName = Globals.DicMan.Get("app.mainwindow.info");                   // Information
            MnuCustomers.DisplayName = Globals.DicMan.Get("app.mainwindow.customers");                  // Customers
            MnuDoctors.DisplayName = Globals.DicMan.Get("app.mainwindow.doctors");                      // Doutores
            MnuAppointmentType.DisplayName = Globals.DicMan.Get("app.mainwindow.appointmenttype");      // Tipos de Consulta

            MnuManage.DisplayName = Globals.DicMan.Get("app.mainwindow.management");                // Management
            MnuManageWelcome.DisplayName = Globals.DicMan.Get("app.mainwindow.info");                   // Information
            MnuAppointments.DisplayName = Globals.DicMan.Get("app.mainwindow.appointments");            // Tipos de Consulta


            //MnuReports.DisplayName = Globals.DicMan.Get(35);                                    // Reports

            System.Threading.Thread.CurrentThread.CurrentCulture = Globals.DicMan.ActiveLanguage.CultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = Globals.DicMan.ActiveLanguage.CultureInfo;
            this.Language = System.Windows.Markup.XmlLanguage.GetLanguage(Globals.DicMan.ActiveLanguage.CultureInfo.IetfLanguageTag);

        }

        private void LoadUserSettings()
        {
            bool save = false;

            // languague
            try
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(Globals.LoggedUser.Language);
                Dictionary.Language userCulter = Globals.DicMan.ActiveLanguage;
                foreach (Dictionary.Language item in Globals.DicMan.AvailableLanguages)
                {
                    if (item.CultureInfo.ToString() == culture.ToString())
                    {
                        userCulter = item;
                        break;
                    }

                }
                if (userCulter != Globals.DicMan.ActiveLanguage)
                    Globals.DicMan.SetActive(userCulter);
                System.Threading.Thread.CurrentThread.CurrentCulture = userCulter.CultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = userCulter.CultureInfo;
            }
            catch (Exception ex01)
            {
                Globals.LogError(ex01);
            }

            // theme
            try
            {

                string[] parms = Globals.LoggedUser.Theme.Split('|');
                Globals.Log("MainWindow", string.Format("Load Theme: {0}", parms[0]));
                var context = new FirstFloor.ModernUI.Presentation.Link() { DisplayName = parms[0], Source = new Uri(parms[1], UriKind.Relative) };
                AppearanceManager.Current.ThemeSource = context.Source;
            }
            catch
            {
                Globals.Log("MainWindow", "Theme failed. Setteing dark as default");

                AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
                Globals.LoggedUser.Theme = string.Format("dark|{0}", AppearanceManager.DarkThemeSource.ToString());
                save = true;
            }


            // UI color
            try
            {
                int color = Globals.LoggedUser.Accentcolor;
                Globals.Log("MainWindow", string.Format("Load AccentColor: {0}", color));
                Color AccentColor = Globals.GetColorFromInt(color);
                AppearanceManager.Current.AccentColor = AccentColor;

                ResourceDictionary resources = Application.Current.Resources; // If in a Window/UserControl/etc
                resources["UserAccent"] = new SolidColorBrush(AccentColor);
            }
            catch
            {
                Globals.Log("MainWindow", "AccentColor failed. Setting green as default.");

                Color AccentColor = Color.FromRgb(0x60, 0xa9, 0x17);   // green                
                AppearanceManager.Current.AccentColor = AccentColor;
                Globals.LoggedUser.Accentcolor = Globals.GetIntFromColor(AccentColor);
                save = true;
            }


            // Save User settings
            if (save)
                Globals.Db.SetUser(new DAL.DataModel.User[] { Globals.LoggedUser });

        }
    }
}
