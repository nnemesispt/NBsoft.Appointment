using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NBsoft.Appointment.WPF.Charts.Line
{
    /// <summary>
    /// Interaction logic for LineChart.xaml
    /// </summary>
    public partial class LineChart : UserControl
    {
        private ObservableCollection<PointSegment> values;
        List<string> labels;

        public List<string> Labels
        {
            get { return labels; }
            set { labels = value; BuildChart(); }
        }

        public LineChart()
        {
            InitializeComponent();
        }

        public string ChartTitle
        {
            get { return (string)GetValue(ChartTitleProperty); }
            set { SetValue(ChartTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChartTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartTitleProperty =
            DependencyProperty.Register("ChartTitle", typeof(string), typeof(LineChart), new PropertyMetadata("Title"));

        public ObservableCollection<PointSegment> ItemList
        {
            get
            {
                return values;
            }

            set
            {
                List<PointSegment> newvalues = (from i in value
                                                select new PointSegment()
                                                {
                                                    Color = i.Color,
                                                    Name = i.Name,
                                                    Value = i.Value,
                                                    LineIndex = i.LineIndex
                                                }).ToList();
                values = new ObservableCollection<PointSegment>(newvalues);
                Line.Data = values;
                foreach (var v in values)
                {
                    v.PropertyChanged += V_PropertyChanged;
                }
                BuildChart();
            }
        }

        public double LineWidth
        {
            get { return Line.Width; }
            set { Line.Width = value; }
        }

        public double LineHeight
        {
            get { return Line.Height; }
            set { Line.Height = value; }
        }

        public double LeftMargin
        {
            get { return Line.LeftMargin; }
            set { Line.LeftMargin = value; }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (values != null)
            {
                PnlLabel.Children.Clear();
                var Lines = (from v in values
                             select v.LineIndex).Distinct();

                foreach (int line in Lines)
                {
                    try
                    {
                        double LineSum = (from v in values
                                          where v.LineIndex == line
                                          select v.Value).Sum();

                        Color color = values.Where(m => m.LineIndex == line).FirstOrDefault().Color;
                        string label = "";
                        if (labels != null && labels.Count >= line + 1)
                            label = labels[line];
                        ChartLabel l = new ChartLabel() { LabelText = label, LabelColor = new SolidColorBrush(color) };
                        l.ToolTip = string.Format("{0}: {1:F2} ", label, LineSum);
                        PnlLabel.Children.Add(l);
                    }
                    catch { continue; }

                }

                //    foreach (PointSegment ps in values)
                //{
                //    ChartLabel l = new ChartLabel() { LabelText = string.Format("{0} ({1:F2})", ps.Name, ps.Value), LabelColor = new SolidColorBrush(ps.Color) };
                //    PnlLabel.Children.Add(l);
                //}
            }
        }

        private void V_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => { InvalidateVisual(); }));
        }

        public void BuildChart()
        {
            Line.IndexNames = Labels == null ? null : Labels.ToArray();
            Dispatcher.Invoke(new Action(() => { InvalidateVisual(); }));
        }
    }
}
