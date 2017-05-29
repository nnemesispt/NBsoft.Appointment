using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace NBsoft.Appointment.WPF.Charts.Line
{
    public class PointSegment : INotifyPropertyChanged, IChartItem
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Color color;
        string name;
        double value;
        int lineIndex;

        public Color Color { get { return color; } set { color = value; OnPropertyChanged(nameof(Color)); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public double Value { get { return value; } set { this.value = value; OnPropertyChanged(nameof(Value)); } }
        public int LineIndex { get { return lineIndex; } set { this.lineIndex = value; OnPropertyChanged(nameof(LineIndex)); } }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
