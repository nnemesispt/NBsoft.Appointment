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

namespace NBsoft.Appointment.WPF.Controls
{
    /// <summary>
    /// Interaction logic for Taskbar.xaml
    /// </summary>
    public partial class Taskbar : UserControl
    {
        public bool IsAddVisible { get { return BtnAdd.Visibility == Visibility.Visible; } set { BtnAdd.Visibility = value ? Visibility.Visible : Visibility.Collapsed; } }
        public bool IsAddEnabled { get { return BtnAdd.IsEnabled; } set { BtnAdd.IsEnabled = value; } }

        public bool IsSaveVisible { get { return BtnSave.Visibility == Visibility.Visible; } set { BtnSave.Visibility = value ? Visibility.Visible : Visibility.Collapsed; } }
        public bool IsSaveEnabled { get { return BtnSave.IsEnabled; } set { BtnSave.IsEnabled = value; } }

        public bool IsDeleteVisible { get { return BtnDelete.Visibility == Visibility.Visible; } set { BtnDelete.Visibility = value ? Visibility.Visible : Visibility.Collapsed; } }
        public bool IsEditVisible { get { return BtnEdit.Visibility == Visibility.Visible; } set { BtnEdit.Visibility = value ? Visibility.Visible : Visibility.Collapsed; } }
        public bool IsSearchVisible { get { return BtnSearch.Visibility == Visibility.Visible; } set { BtnSearch.Visibility = SearchLine.Visibility = TxtSearch.Visibility = (value ? Visibility.Visible : Visibility.Collapsed); } }
        public bool IsPrintVisible { get { return BtnPrint.Visibility == Visibility.Visible; } set { BtnPrint.Visibility = PrinterLine.Visibility = (value ? Visibility.Visible : Visibility.Collapsed); } }


        public Taskbar()
        {
            InitializeComponent();
            IsPrintVisible = false;
            IsEditVisible = false;
        }

        private void OnAdd(EventArgs e)
        {
            Add?.Invoke(this, e);
        }
        private void OnSave(EventArgs e)
        {
            Save?.Invoke(this, e);
        }
        private void OnEdit(EventArgs e)
        {
            Edit?.Invoke(this, e);
        }
        private void OnDelete(EventArgs e)
        {
            Delete?.Invoke(this, e);
        }
        private void OnPrint(EventArgs e)
        {
            Print?.Invoke(this, e);
        }
        private void OnSearch(Common.TextEventArgs e)
        {
            Search?.Invoke(this, e);
        }




        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            OnAdd(e);
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            OnSave(e);
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            OnEdit(e);
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            OnDelete(e);
        }
        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            OnPrint(e);
        }
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Common.TextEventArgs te = new Common.TextEventArgs(TxtSearch.Text);
            OnSearch(te);
        }

        public event EventHandler Add;
        public event EventHandler Edit;
        public event EventHandler Save;
        public event EventHandler Delete;
        public event EventHandler Print;
        public event EventHandler<Common.TextEventArgs> Search;

        private void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Common.TextEventArgs te = new Common.TextEventArgs(TxtSearch.Text);
                OnSearch(te);
            }
        }


    }
}
