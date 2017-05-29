using NBsoft.Appointment.WPF.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NBsoft.Appointment.WPF
{
    

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static DebugWindow DebugWnd { get; private set; }
        internal static bool ShowConfig { get; private set; }

        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DebugWnd = null;
            bool DebugMode = false;
            ShowConfig = false;
            foreach (var arg in e.Args)
            {
                switch (arg)
                {
                    case "-d":
                        DebugMode = true;
                        break;
                    case "-c":
                        ShowConfig = true;
                        break;
                    default:
                        break;
                }

            }

            if (DebugMode)
            {
                DebugWnd = new DebugWindow();
                DebugWnd.Show();
            }

        }
    }
}
