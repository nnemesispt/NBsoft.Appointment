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
using FirstFloor.ModernUI.Windows.Controls;

namespace NBsoft.Appointment.WPF.Pages
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : UserControl, IContent
    {
        UserVM user;
        DAL.DataModel.User current;
        public UserPage()
        {
            InitializeComponent();
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            throw new NotImplementedException();
        }
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            user = null;
            this.DataContext = null;
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            current = Globals.LoggedUser;
            int pin = 1234;
            int.TryParse(current.PIN, out pin);
            user = new UserVM()
            {
                Id = current.Id,
                Account = current.Logon,
                Firstname = current.Firstname,
                Lastname = current.Lastname,
                Address = current.Address,
                PostalCode = current.PostalCode,
                City = current.City,
                Country = current.Country,
                Language = current.Language,
                Password = current.Password,
                Email = current.Email
            };
            PwbPIN.Password = "";
            this.DataContext = user;

            BBChangePassword.BBCode = BBChangePassword.BBCode.Replace("#app.user.changepassword#", Globals.DicMan.Get("app.user.changepassword"));

            if (current.Logon != "sa")
                BtnAddUser.Visibility = Visibility.Collapsed;
        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {            
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            int pin = 0;

            var users = Globals.Db.GetUser();            
            foreach (var usr in users)
            {
                if (usr.Logon == user.Account && user.Id == 0)
                {

                    ModernDialog.ShowMessage(Globals.DicMan.Get("app.user.alreadyexists"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                    return;
                }

            }

            if (PwbPIN.Password.Length > 0)
            {
                if (!int.TryParse(PwbPIN.Password, out pin))
                {                    
                    ModernDialog.ShowMessage(Globals.DicMan.Get("app.user.pin.mustbenumber"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                    return;
                }
                if (PwbPIN.Password.Length < 3)
                {
                    ModernDialog.ShowMessage(Globals.DicMan.Get("app.user.pin.tooshort"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
                    return;
                }
                Globals.LoggedUser.PIN = PwbPIN.Password;
            }

            current.Logon = user.Account;
            current.Email = user.Email;
            current.Firstname = user.Firstname;
            current.Lastname = user.Lastname;
            current.Address = user.Address;
            current.PostalCode = user.PostalCode;
            current.City = user.City;
            current.Country = user.Country;
            //Globals.LoggedUser.Language = user.Language;
            //Globals.LoggedUser.Password = user.Password;
                        

            try
            {
                Globals.Db.SetUser(new DAL.DataModel.User[] { current });                
                ModernDialog.ShowMessage(Globals.DicMan.Get("app.user.saveok"), Globals.AppName, MessageBoxButton.OK, Globals.MainWnd);
            }
            catch (Exception ex01)
            {
                Globals.LogError(ex01);
                ModernDialog.ShowMessage(ex01.Message, "Err: " + ex01.Source, System.Windows.MessageBoxButton.OK, Globals.MainWnd);
            }


        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            current = new DAL.DataModel.User()
            {
                CreationDate = DateTime.Now,
                Theme = Globals.LoggedUser.Theme,
                Accentcolor = Globals.LoggedUser.Accentcolor,
                Country = Globals.LoggedUser.Country,
                Language = Globals.LoggedUser.Language,                
                Password = Globals.Encrypt("1234"),
                PIN = "1234"
            };
            this.DataContext = null;
            user = new UserVM()
            {
                Account = current.Logon,
                Firstname = current.Firstname,
                Lastname = current.Lastname,
                Address = current.Address,
                PostalCode = current.PostalCode,
                City = current.City,
                Country = current.Country,
                Language = current.Language,
                Password = current.Password,
                Email = current.Email
            };
            PwbPIN.Password = "";
            this.DataContext = user;
            TxtId.IsReadOnly = false;
            TxtId.Focus();

        }
    }
}
