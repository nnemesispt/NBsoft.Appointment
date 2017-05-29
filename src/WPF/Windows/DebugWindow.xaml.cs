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
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        List<Common.LogEventArgs> LogList;

        public DebugWindow()
        {
            InitializeComponent();
            LogList = new List<Common.LogEventArgs>();
            DataContext = LogList;
            Loaded += DebugWindow_Loaded;
        }

        private void DebugWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Globals.Log(this, "Starting Debug Widow", Enums.LogType.Info);

                Globals.Logger.LogEvent += Logger_LogEvent;



            }
            catch (Exception ex01)
            {
                Globals.LogError(ex01);
            }
        }

        private void Logger_LogEvent(object sender, Common.LogEventArgs e)
        {

            LogList.Add(e);
            this.Dispatcher.Invoke(new Action(SetSource));
        }
        private void SetSource()
        {
            LvwLogs.ItemsSource = null;
            this.Title = "Logs:" + LogList.Last().LogDate;
            LvwLogs.ItemsSource = LogList;
        }

    }
}
