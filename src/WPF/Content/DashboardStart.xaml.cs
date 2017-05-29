using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using NBsoft.Appointment.DAL.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NBsoft.Appointment.WPF.Content
{
    /// <summary>
    /// Interaction logic for DashboardStart.xaml
    /// </summary>
    public partial class DashboardStart : UserControl,IContent
    {
        LocalVM viewModel;
        public DashboardStart()
        {
            InitializeComponent();

            //var ap = Globals.Db.GetAppointment();
            //List<DAL.DataModel.Appointment> changed = new List<DAL.DataModel.Appointment>();

            //long lastId = 0;
            //long oldid;
            //for (int i = 0; i < ap.Count(); i++ )
            //{                
            //    oldid = ap[i].Id_Doctor;
            //    ap[i].Id_Doctor = ++lastId;
            //    if (lastId >= 3)
            //        lastId = 0;

            //    if (ap[i].Id_Doctor != oldid)
            //    {
            //        changed.Add(ap[i]);
            //    }
            //    i = i + 2;
            //}
            //if (changed.Count > 0)
            //    Globals.Db.SetAppointment(changed.ToArray());

        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {            
        }
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {         
        }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            GetDataAsync();
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }


        void ShowWait()
        {
            WaitAnimation.IsActive = true;
            WaitGrid.Visibility = Visibility.Visible;
        }
        void HideWait()
        {
            WaitGrid.Visibility = Visibility.Collapsed;
            WaitAnimation.IsActive = false;
        }

        private void GetDataAsync()
        {
            ShowWait();

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork; ;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            viewModel = new LocalVM();
            try
            {
                var customers = Globals.Db.GetCustomer();

                DateTime today = DateTime.Today;
                DateTime maxlimit = today.AddMonths(6);
                viewModel.UpcomingAppointment = (from c in customers
                                             where c.NextAppointment.HasValue &&
                                             c.NextAppointment >= today && c.NextAppointment <= maxlimit
                                             orderby c.NextAppointment
                                             select c).ToArray();

                DateTime last3 = new DateTime(today.Year - 3, 1, 1);
                viewModel.Last3YearsApp = Globals.Db.GetAppointment(last3, today);


                viewModel.DoctorList = Globals.Db.GetDoctor();
            }
            catch(Exception ex01)
            {
                Globals.LogError(ex01);
                viewModel.UpcomingAppointment = new Customer[0];
            }


        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideWait();
            if (e.Error != null)
            {
                Globals.LogError(e.Error);
                ModernDialog.ShowMessage(e.Error.Message, e.Error.Source, MessageBoxButton.OK, Globals.MainWnd);                
            }
            this.DataContext = viewModel;
            BuildCharts();
        }

        private void BuildCharts()
        {
            #region LineChartAppointmentsCount
            try
            {
                Globals.Log("DashboardStart", "LineChartAppointmentsCount Started");
                var App0Year = viewModel.Last3YearsApp.Where(m => m.AppointmentDate.Year == DateTime.Now.Year);
                var App0YearGrouped = from d in App0Year
                                            group d by new { month = d.AppointmentDate.Month } into g
                                             select new { dt = g.Key.month, count = g.Count() };

                var App1Year = viewModel.Last3YearsApp.Where(m => m.AppointmentDate.Year == DateTime.Now.Year - 1);
                var App1YearGrouped = from d in App1Year
                                         group d by new { month = d.AppointmentDate.Month } into g
                                          select new { dt = g.Key.month, count = g.Count() };

                var App2Year = viewModel.Last3YearsApp.Where(m => m.AppointmentDate.Year == DateTime.Now.Year - 2);
                var App2YearGrouped = from d in App2Year
                                      group d by new { month = d.AppointmentDate.Month } into g
                                      select new { dt = g.Key.month, count = g.Count() };

                if (App0YearGrouped.Count() > 0 || App1YearGrouped.Count() > 0 || App2YearGrouped.Count()>0)
                {
                    System.Collections.ObjectModel.ObservableCollection<Charts.Line.PointSegment> appsTotal = new System.Collections.ObjectModel.ObservableCollection<Charts.Line.PointSegment>();
                    string Y2Label="";
                    string Y1Label="";
                    string Y0Label="";
                    if (App2YearGrouped.Count() > 0)
                        Y2Label = App2Year.First().AppointmentDate.Year.ToString();
                    if (App1YearGrouped.Count() > 0)
                        Y1Label = App1Year.First().AppointmentDate.Year.ToString();
                    if (App0YearGrouped.Count() > 0)
                        Y0Label = App0Year.First().AppointmentDate.Year.ToString();

                    for (int i = 1; i <= 12; i++)
                    {
                        string month = new DateTime(DateTime.Today.Year, i, 1).ToString("MMM");
                        int year2Count = 0;
                        if (App2YearGrouped.Count()>0)
                            year2Count  = App2YearGrouped.Where(m => m.dt == i).Count() > 0 ? App2YearGrouped.Where(m => m.dt == i).First().count : 0;
                        int year1Count = 0;
                        if (App1YearGrouped.Count() > 0)
                            year1Count = App1YearGrouped.Where(m => m.dt == i).Count() > 0 ? App1YearGrouped.Where(m => m.dt == i).First().count : 0;
                        int year0Count = 0;
                        if (App0YearGrouped.Count() > 0)
                            year0Count = App0YearGrouped.Where(m => m.dt == i).Count() > 0 ? App0YearGrouped.Where(m => m.dt == i).First().count : 0;

                        if (App2YearGrouped.Count() > 0)
                            appsTotal.Add(new Charts.Line.PointSegment() { Color = SettingsAppearanceViewModel.AccentColors[2], Name = month, Value = year2Count, LineIndex = 0 });
                        if (App1YearGrouped.Count() > 0)
                            appsTotal.Add(new Charts.Line.PointSegment() { Color = SettingsAppearanceViewModel.AccentColors[4], Name = month, Value = year1Count, LineIndex = 1 });
                        if (App0YearGrouped.Count() > 0)
                            appsTotal.Add(new Charts.Line.PointSegment() { Color = SettingsAppearanceViewModel.AccentColors[11], Name = month, Value = year0Count, LineIndex = 2 });
                    }
                    LineChartAppointmentsCount.ItemList = appsTotal;
                    LineChartAppointmentsCount.Labels = new List<string>() { Y2Label, Y1Label, Y0Label };
                }
            }
            finally { Globals.Log("DashboardStart", "LineChartAppointmentsCount Finished"); }
            #endregion

            #region PieAppointmentDoctor
            try
            {
                Globals.Log("DashboardStart", "PieAppointmentDoctor Started");
                List<Charts.Pie.PieSegment> pieJobRoute = new List<Charts.Pie.PieSegment>();
                var lastYear = viewModel.Last3YearsApp .Where(m => m.AppointmentDate >= DateTime.Today.AddYears(-1));

                int i = 0;
                foreach (var doc in viewModel.DoctorList)
                {

                    Color c = SettingsAppearanceViewModel.AccentColors[i];
                    i = i + 2;
                    if (i >= SettingsAppearanceViewModel.AccentColors.Length)
                        i = 1;

                    var doctorApps = from ad in lastYear
                                     where ad.Id_Doctor == doc.Id
                                     select ad;
                    int appCount = 0;
                    if (doctorApps != null && doctorApps.Count() > 0)
                        appCount = doctorApps.Count();

                    pieJobRoute.Add(new Charts.Pie.PieSegment() { Name = doc.Name, Value = appCount, Color = c });
                }
                PieAppointmentDoctor.ItemList = pieJobRoute.ToArray();
            }
            finally { Globals.Log("DashboardStart", "PieAppointmentDoctor Finished"); }
            #endregion


        }

        private void Tskbar_Search(object sender, Common.TextEventArgs e)
        {
            DG1.SelectionMode = DataGridSelectionMode.Extended;
            var res = from c in viewModel.UpcomingAppointment
                      where c.Name.ToLower().Contains(e.Text.ToLower())
                      select c;
            DG1.SelectedItems.Clear();
            if (res.Count() > 0)
            {
                foreach (var item in DG1.Items)
                {
                    if (res.Contains(item))
                        DG1.SelectedItems.Add(item);
                }
            }
                

        }

        private void Tskbar_Print(object sender, EventArgs e)
        {
            string file = System.IO.Path.Combine(Globals.CommonPath, string.Format("{0}.csv", DateTime.Now.ToString("yyyymmdd_hhmmss")));
            try
            {
                DG1.ExportUsingRefection(file);

                System.Diagnostics.Process.Start(file);
            }
            catch (Exception ex01)
            {
                ModernDialog.ShowMessage(ex01.Message, ex01.Source, MessageBoxButton.OK, Globals.MainWnd);
            }

        }

        private void Tskbar_Edit(object sender, EventArgs e)
        {

        }

        public class LocalVM: INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private DAL.DataModel.Customer[] upcomingAppointment;
            private DAL.DataModel.Appointment[] last3YearsApp;
            private DAL.DataModel.Doctor[] doctorList;

            public DAL.DataModel.Customer[] UpcomingAppointment { get { return upcomingAppointment; } set { upcomingAppointment = value; OnPropertyChanged(nameof(UpcomingAppointment)); } }
            public DAL.DataModel.Appointment[] Last3YearsApp { get { return last3YearsApp; } set { last3YearsApp = value; OnPropertyChanged(nameof(Last3YearsApp)); } }
            public DAL.DataModel.Doctor[] DoctorList { get { return doctorList; } set { doctorList = value; OnPropertyChanged(nameof(DoctorList)); } }




            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
                
       
    }
}
