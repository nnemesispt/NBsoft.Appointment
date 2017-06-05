using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NBsoft.Appointment.WPF.Commands
{
    class OpenEULACommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = System.IO.Path.Combine(Globals.InstallPath, "LICENSE.txt"),
                UseShellExecute = true
            };

            try { System.Diagnostics.Process.Start(psi); }
            catch (Exception ex01) { Globals.LogError(ex01); }
        }
        private void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }
    }
}
