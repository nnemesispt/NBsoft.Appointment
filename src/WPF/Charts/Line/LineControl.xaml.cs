using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NBsoft.Appointment.WPF.Charts.Line
{
    /// <summary>
    /// Interaction logic for LineControl.xaml
    /// </summary>
    public partial class LineControl : UserControl
    {
        ObservableCollection<PointSegment> values;
        Dictionary<Path, PointSegment> pathDictionary = new Dictionary<Path, PointSegment>();


        public ObservableCollection<PointSegment> Data
        {
            get { return values; }
            set
            {
                values = value;
                values.CollectionChanged += Values_CollectionChanged;
                foreach (var v in values)
                {
                    v.PropertyChanged += PointSegment_PropertyChanged;
                }
                ResetGraph();
            }
        }
        public bool DrawLines { get; set; }
        public double LeftMargin { get; set; }
        public double BottomMargin { get; set; }
        public string[] IndexNames { get; set; }


        private void Values_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ResetGraph();
        }

        public LineControl()
        {
            InitializeComponent();
            DrawLines = true;
            BottomMargin = 20;
            LeftMargin = 50;
        }


        void AddPathToDictionary(Path path, PointSegment ps)
        {
            pathDictionary.Add(path, ps);
            path.MouseEnter += Path_MouseEnter;
            //path.MouseMove += Path_MouseMove;
        }

        void ClearPathDictionary()
        {
            foreach (Path path in pathDictionary.Keys)
            {
                path.MouseEnter -= Path_MouseEnter;
                //path.MouseMove -= Path_MouseMove;
            }
            pathDictionary.Clear();
        }

        private void BuildGraph()
        {

            ClearPathDictionary();
            drawingCanvas.Children.Clear();

            if (Data != null)
            {
                double total = Data.GetTotal();
                double maxValue = Data.Count > 0 ? Data.Max(m => m.Value) : 0;

                if (total > 0)
                {
                    int c = 0;

                    #region  Draw Scale
                    Path scalePath = new Path();
                    GeometryGroup sg = new GeometryGroup();
                    sg.Children.Add(new LineGeometry(new Point(LeftMargin, 0), new Point(LeftMargin, Height - BottomMargin)));
                    sg.Children.Add(new LineGeometry(new Point(LeftMargin, Height - BottomMargin), new Point(Width, Height - BottomMargin)));
                    scalePath.StrokeThickness = 1;
                    scalePath.Stroke = Foreground;
                    double xStart = LeftMargin + 10;
                    double xEnd = Width - 10;
                    double xScale = xEnd - xStart;

                    double yStart = Height - BottomMargin;
                    double yEnd = 10;
                    double yScale = yEnd - yStart;

                    Dictionary<string, Point> LineMatrix = new Dictionary<string, Point>();
                    //List<Point> GraphPoints = new List<Point>();
                    int[] lines = (from l in Data
                                   select l.LineIndex).Distinct().ToArray();
                    for (int i = 0; i < lines.Count(); i++)
                    {
                        List<PointSegment> linePoints = (from l in Data
                                                         where l.LineIndex == lines[i]
                                                         select l).ToList();
                        c = 0;
                        int totalItems = linePoints.Count - 1;
                        foreach (PointSegment ps in linePoints)
                        {
                            double x = xStart + (((xScale) / totalItems) * c++);
                            double y = yStart + (((yScale) * ps.Value) / maxValue);
                            Point newPoint = new Point(x, y);
                            //GraphPoints.Add(newPoint);
                            LineMatrix.Add(string.Format("{0};{1}", lines[i], c), newPoint);
                            if (i == 0)
                                sg.Children.Add(new LineGeometry(new Point(x, Height - BottomMargin), new Point(x, Height - BottomMargin - 5))); // X Scale Lines
                        }
                    }
                    scalePath.Data = sg;
                    drawingCanvas.Children.Add(scalePath);

                    Path yscalePath = new Path();
                    GeometryGroup ysg = new GeometryGroup();
                    yscalePath.StrokeThickness = 1;
                    yscalePath.Stroke = Foreground;
                    yscalePath.StrokeDashArray = new DoubleCollection(new double[] { 4, 8 });
                    double yMiddle = yStart + (((yScale) * (maxValue / 2)) / maxValue);
                    ysg.Children.Add(new LineGeometry(new Point(LeftMargin, yEnd), new Point(xEnd, yEnd))); // Y Scale Top Line
                    ysg.Children.Add(new LineGeometry(new Point(LeftMargin, yMiddle), new Point(xEnd, yMiddle))); // Y Scale Middle Line
                    //ysg.Children.Add(new LineGeometry(new Point(LeftMargin, yStart), new Point(xEnd, yStart))); // Y Scale Bottom Line
                    yscalePath.Data = ysg;
                    drawingCanvas.Children.Add(yscalePath);
                    #endregion

                    #region  Draw text
                    Path textPath = new Path();
                    GeometryGroup tg = new GeometryGroup();
                    textPath.Fill = Foreground;
                    Typeface[] tfaces = this.FontFamily.GetTypefaces().ToArray();
                    Typeface tface = tfaces.Where(m => m.Weight == this.FontWeight && m.Style == this.FontStyle && m.Stretch == this.FontStretch).First();

                    List<PointSegment> linePoints1 = (from l in Data
                                                      where l.LineIndex == 0
                                                      select l).ToList();

                    for (int i = 0; i < linePoints1.Count; i++)
                    {
                        PointSegment ps = linePoints1[i];
                        //Point p = GraphPoints[i];
                        string key = string.Format("0;{0}", i + 1);
                        Point p = LineMatrix[key];
                        FormattedText text = new FormattedText(ps.Name,
                           System.Globalization.CultureInfo.CurrentCulture,
                           FlowDirection.LeftToRight,
                           tface,
                           this.FontSize,
                           Foreground);
                        Point startPoint;
                        if (i < Data.Count - 1)
                            startPoint = new Point(p.X - (text.Width / 2), Height - BottomMargin);
                        else
                            startPoint = new Point(p.X - text.Width + 5, Height - BottomMargin);

                        Geometry textgeometry = text.BuildGeometry(startPoint);
                        tg.Children.Add(textgeometry);
                    }

                    FormattedText textMax = new FormattedText(maxValue.ToString("F2", System.Globalization.CultureInfo.CurrentCulture),
                           System.Globalization.CultureInfo.CurrentCulture,
                           FlowDirection.LeftToRight,
                           tface,
                           this.FontSize,
                           Foreground);
                    Geometry maxtextgeometry = textMax.BuildGeometry(new Point(LeftMargin - textMax.Width - 5, yEnd));
                    tg.Children.Add(maxtextgeometry);

                    FormattedText textMiddle = new FormattedText((maxValue / 2).ToString("F2", System.Globalization.CultureInfo.CurrentCulture),
                           System.Globalization.CultureInfo.CurrentCulture,
                           FlowDirection.LeftToRight,
                           tface,
                           this.FontSize,
                           Foreground);
                    Geometry middletextgeometry = textMiddle.BuildGeometry(new Point(LeftMargin - textMiddle.Width - 5, yMiddle));
                    tg.Children.Add(middletextgeometry);

                    textPath.Data = tg;
                    drawingCanvas.Children.Add(textPath);
                    #endregion

                    #region Draw Graph Lines
                    if (DrawLines)
                    {
                        var lines1 = (from l in Data
                                      select l.LineIndex).Distinct();

                        foreach (var item in lines1)
                        {
                            List<PointSegment> linePoints = (from l in Data
                                                             where l.LineIndex == item
                                                             select l).ToList();
                            Point? Lastpoint = null;
                            for (int i = 0; i < linePoints.Count; i++)
                            {
                                PointSegment ps = linePoints[i];
                                string key = string.Format("{0};{1}", item, i + 1);
                                Point p = LineMatrix[key];
                                if (Lastpoint != null)
                                {
                                    Path linePath = new Path();
                                    linePath.Data = new LineGeometry(p, Lastpoint.Value);
                                    linePath.Stroke = new SolidColorBrush(ps.Color);
                                    linePath.StrokeThickness = 1;
                                    drawingCanvas.Children.Add(linePath);
                                }
                                Lastpoint = p;
                            }
                        }
                    }
                    #endregion

                    var lines2 = (from l in Data
                                  select l.LineIndex).Distinct();
                    List<string> indexNames = new List<string>();
                    foreach (var item in lines2)
                    {
                        indexNames.Add("Line " + item.ToString());


                        List<PointSegment> linePoints2 = (from l in Data
                                                          where l.LineIndex == item
                                                          select l).ToList();

                        for (int i = 0; i < linePoints2.Count; i++)
                        {
                            PointSegment ps = linePoints2[i];

                            //Point p =  GraphPoints[i];
                            string key = string.Format("{0};{1}", item, i + 1);
                            Point p = LineMatrix[key];

                            Path valuePath = new Path();
                            if (ps.Value == 0)
                                valuePath.Data = new RectangleGeometry(new Rect(p.X - 1, p.Y - 1, 2, 2));
                            else
                                valuePath.Data = new RectangleGeometry(new Rect(p.X - 4, p.Y - 4, 8, 8));
                            valuePath.Fill = new SolidColorBrush(ps.Color);
                            drawingCanvas.Children.Add(valuePath);
                            AddPathToDictionary(valuePath, ps);
                        }
                    }
                    if (IndexNames == null)
                        IndexNames = indexNames.ToArray();


                }

            }
        }

        void ResetGraph()
        {
            Dispatcher.Invoke(new Action(BuildGraph));
        }

        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            PointSegment seg = pathDictionary[sender as Path];
            List<PointSegment> CurrentLine = (from l in Data
                                              where l.LineIndex == seg.LineIndex
                                              select l).ToList();
            var total = CurrentLine.Sum(m => m.Value);
            string LineName = this.IndexNames[seg.LineIndex];

            popupData.Text = string.Format("{3}, {0}: {1:F2} ({2:F2}%)", seg.Name, seg.Value, ((seg.Value / total) * 100), LineName);
        }
        private void PointSegment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ResetGraph();
        }


    }
}
