using FirstFloor.ModernUI.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace NBsoft.Appointment.WPF.Content
{
    /// <summary>
    /// Interaction logic for TableSearch.xaml
    /// </summary>
    public partial class TableSearch : UserControl, IContent
    {
        LocalVM viewModel;
        List<TableItem> OriginalList;
        public TableSearch()
        {
            InitializeComponent();
        }

        public void Initialize(TableItem[] tableToSearch)
        {
            

        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
                        
        }
        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            this.DataContext = null;

            DAL.DataModel.AppointmentType[] appointmentType = Pages.AppointmentsPage.ActivePage.TableList as DAL.DataModel.AppointmentType[];
            if (appointmentType != null)
            {
                viewModel = new LocalVM();
                viewModel.ItemList = (from c in appointmentType
                                      select new TableItem()
                                      {
                                          Id = c.Id,
                                          Name = c.Name,
                                          Tag = c
                                      }).ToArray();

                OriginalList = viewModel.ItemList.ToList();
                this.DataContext = viewModel;
                return;
            }

            DAL.DataModel.Doctor[] doctors = Pages.AppointmentsPage.ActivePage.TableList as DAL.DataModel.Doctor[];
            if (doctors != null)
            {
                viewModel = new LocalVM();
                viewModel.ItemList = (from c in doctors
                                      select new TableItem()
                                      {
                                          Id = c.Id,
                                          Name = c.Name,
                                          Tag = c
                                      }).ToArray();

                OriginalList = viewModel.ItemList.ToList();
                this.DataContext = viewModel;
                return;
            }

            DAL.DataModel.Customer[] customers = Pages.AppointmentsPage.ActivePage.TableList as DAL.DataModel.Customer[];
            if (customers != null)
            {
                viewModel = new LocalVM();
                viewModel.ItemList = (from c in customers
                                      select new TableItem()
                                      {
                                          Id = c.Id,
                                          Name = c.Name,
                                          Tag = c
                                      }).ToArray();

                OriginalList = viewModel.ItemList.ToList();
                this.DataContext = viewModel;
                return;
            }
            
        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }
        private void Taskbar_Search(object sender, Common.TextEventArgs e)
        {
            DG1.ItemsSource = null;            
            if (e.Text == "")
                viewModel.ItemList = OriginalList.ToArray();
            else
                viewModel.ItemList = (from i in OriginalList
                                      where i.Id.ToString().Contains(e.Text) ||
                                      i.Name.ToLower().Contains(e.Text.ToLower())
                                      select i).ToArray();

            DG1.ItemsSource = viewModel.ItemList;

        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (DG1.SelectedItem == null)
                return;

            Pages.AppointmentsPage.ActivePage.TableListOk((TableItem)DG1.SelectedItem);

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var nav = Globals.MainWnd.LinkNavigator;
            var cmdBack = nav.Commands;

            BBCodeBlock bbBlock = new BBCodeBlock();
            bbBlock.LinkNavigator.Navigate(new Uri("cmd://browseback", UriKind.Absolute), this, NavigationHelper.FrameSelf);

            Console.WriteLine("ss");
        }

        

        public class TableItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            long id;
            string name;
            object tag;

            public long Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
            public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
            public object Tag { get { return tag; } set { tag = value; OnPropertyChanged(nameof(Tag)); } }


            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        public class LocalVM
        {
            public TableItem[] ItemList { get; set; }
        }
    }
}
