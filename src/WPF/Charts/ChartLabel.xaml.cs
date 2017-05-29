using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NBsoft.Appointment.WPF.Charts
{
    /// <summary>
    /// Interaction logic for ChartLabel.xaml
    /// </summary>
    public partial class ChartLabel : UserControl
    {
        public Brush LabelColor
        {
            get { return (Brush)GetValue(LabelColorProperty); }
            set { SetValue(LabelColorProperty, value); Rect1.Fill = value; }
        }

        // Using a DependencyProperty as the backing store for LabelColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelColorProperty =
            DependencyProperty.Register("LabelColor", typeof(Brush), typeof(ChartLabel), new PropertyMetadata(Brushes.White));




        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); LblText.Content = value; }
        }
        // Using a DependencyProperty as the backing store for LabelText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(ChartLabel), new PropertyMetadata(""));



        public ChartLabel()
        {
            InitializeComponent();
        }
    }
}
