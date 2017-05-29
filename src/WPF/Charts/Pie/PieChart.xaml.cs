using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;

namespace NBsoft.Appointment.WPF.Charts.Pie
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class PieChart : UserControl, IChart
    {
        public ObservableCollection<PieSegment> values;

        public Brush PopupBrush
        {
            get { return Pie.PopupBrush; }
            set { Pie.PopupBrush = value; }
        }



        public string ChartTitle
        {
            get { return (string)GetValue(ChartTitleProperty); }
            set { SetValue(ChartTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChartTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartTitleProperty =
            DependencyProperty.Register("ChartTitle", typeof(string), typeof(PieChart), new PropertyMetadata("Title"));


        public PieChart()
        {
            InitializeComponent();
            //ToolTipService.ShowDurationProperty.OverrideMetadata(
            //    typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
        }


        void PieSegment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => { InvalidateVisual(); }));
        }

        public double PieWidth
        {
            get { return Pie.Width; }
            set { Pie.Width = value; }
        }

        public double PieHeight
        {
            get { return Pie.Height; }
            set { Pie.Height = value; }
        }

        public IChartItem[] ItemList
        {
            get
            {
                return values.ToArray();
            }

            set
            {
                List<PieSegment> newvalues = (from i in value
                                              select new PieSegment()
                                              {
                                                  Color = i.Color,
                                                  Name = i.Name,
                                                  Value = i.Value
                                              }).ToList();
                values = new ObservableCollection<PieSegment>(newvalues);
                Pie.Data = values;
                foreach (var v in values)
                {
                    v.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PieSegment_PropertyChanged);
                }
                BuildChart();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (values != null)
            {
                //double height = values.Count * 20;
                //double top = (Height - height) / 2;
                //foreach (PieSegment ps in values)
                //{
                //    dc.DrawRectangle(ps.SolidBrush, null, new Rect(Pie.Width + 10, top, 8, 8));
                //    dc.DrawText(GetFormattedText(ps.Name + " (" + ps.Value + ")", 12, Brushes.Black), new Point(Pie.Width + 20, top));
                //    top += 20;
                //}
                PnlLegend.Children.Clear();
                foreach (PieSegment ps in values)
                {
                    ChartLabel l = new ChartLabel() { LabelText = string.Format("({1:F2}) {0}", ps.Name, ps.Value), LabelColor = ps.SolidBrush };
                    PnlLegend.Children.Add(l);
                }
            }
        }


        public FormattedText GetFormattedText(string textToFormat, double fontSize, Brush brush)
        {
            Typeface typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            return new FormattedText(textToFormat, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, typeface, fontSize, brush);
        }

        public void BuildChart()
        {
            Dispatcher.Invoke(new Action(() => { InvalidateVisual(); }));
        }
    }
}
