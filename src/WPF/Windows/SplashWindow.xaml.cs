using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();

            StartupAsync();
        }

        private void StartupAsync()
        {
            BackgroundWorker StartupBW = new BackgroundWorker();
            StartupBW.DoWork += StartupBW_DoWork;
            StartupBW.RunWorkerCompleted += StartupBW_RunWorkerCompleted;
            StartupBW.RunWorkerAsync();
        }

        private void StartupBW_DoWork(object sender, DoWorkEventArgs e)
        {
            //System.Threading.Thread.Sleep(500);
            this.Dispatcher.Invoke(new Action(Globals.Startup));
        }

        private void StartupBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, this);
            }            
            this.Close();

        }
    }
}
