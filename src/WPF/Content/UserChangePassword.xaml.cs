using FirstFloor.ModernUI.Windows;
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

namespace NBsoft.Appointment.WPF.Content
{
    /// <summary>
    /// Interaction logic for UserChangePassword.xaml
    /// </summary>
    public partial class UserChangePassword : UserControl, IContent
    {
        public UserChangePassword()
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
            TxtCurrentPassword.Password = "";
            TxtNewPassword.Password = "";
            TxtPasswordConfirmation.Password = "";
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCurrentPassword.Password == "" || TxtNewPassword.Password == "" || TxtPasswordConfirmation.Password == "")
                return;

            string p = Globals.Encrypt(TxtCurrentPassword.Password);
            if (Globals.LoggedUser.Password == p)
            {
                string p1 = TxtNewPassword.Password;
                string p2 = TxtPasswordConfirmation.Password;

                if (p1 == p2)
                {
                    if (p1.Length < 5)
                    {
                        ModernDialog.ShowMessage(Globals.DicMan.Get("app.user.changepassword.tooshort"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                        return;
                    }

                    Globals.LoggedUser.Password = Globals.Encrypt(p1);
                    try {
                        Globals.Db.SetUser(new DAL.DataModel.User[] { Globals.LoggedUser });
                        ModernDialog.ShowMessage(Globals.DicMan.Get("app.user.changepassword.applyok"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                        NavigationCommands.BrowseBack.Execute(null, this);
                    }                    
                    catch (Exception ex01)
                    {
                        Globals.LogError(ex01);
                        ModernDialog.ShowMessage(ex01.Message, "Err: " + ex01.Source, System.Windows.MessageBoxButton.OK, Globals.MainWnd);
                    }
                    
                }
                else
                    ModernDialog.ShowMessage(Globals.DicMan.Get("app.user.changepassword.mismatch"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);

            }
            else
                ModernDialog.ShowMessage(Globals.DicMan.Get("app.user.changepassword.authfailed"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);

        }
    }
}
