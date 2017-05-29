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
using System.Windows.Shapes;

namespace NBsoft.Appointment.WPF.Windows
{
    /// <summary>
    /// Interaction logic for LogonWindow.xaml
    /// </summary>
    public partial class LogonWindow : Window
    {
        public static DAL.DataModel.User Authenticate(Window owner, DAL.DataModel.User[] users)
        {
            if (users== null || users.Length<1)
                return null;

            LogonWindow lwnd = new LogonWindow();
            lwnd.userList = users;
            lwnd.Owner = owner;
            bool? res = lwnd.ShowDialog();
            if (res.HasValue && res.Value == true)
                return lwnd.authenticatedUser;
            else
                return null;
        }

        DAL.DataModel.User[] userList;
        DAL.DataModel.User authenticatedUser;

        public LogonWindow()
        {
            InitializeComponent();
            authenticatedUser = null;
            TxtAccount.Focus();
        }

        private void TxtAccount_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                TxtPass.Focus();
        }

        private void TxtPass_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Auth();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Auth();
        }

        private void Auth()
        {
            string UserAccount = TxtAccount.Text;
            string UserPassword = TxtPass.Password;
            if (UserAccount == "")
            {
                return;
            }
            if (UserPassword == "")
            {
                return;
            }

            var u = userList.Where(m => m.Logon == UserAccount).FirstOrDefault();
            if (u == null)
            {
                TxbAuthfailed.Text = Globals.DicMan.Get("app.logonwindow.invaliduser");                
            }
            else
            {
                if (u.Password == Globals.Encrypt(UserPassword))
                {
                    authenticatedUser = u;
                    this.DialogResult = true;
                }
                else
                {
                    TxbAuthfailed.Text = Globals.DicMan.Get("app.logonwindow.authfailed");
                }
            }

        }

        
    }
}
