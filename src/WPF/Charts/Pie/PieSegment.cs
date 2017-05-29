using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace NBsoft.Appointment.WPF.Charts.Pie
{
    public class PieSegment : INotifyPropertyChanged, IChartItem
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string name;
        double value;
        Brush gradientBrush;
        Brush solidBrush;
        Color color;

        public double Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                gradientBrush = new LinearGradientBrush(MakeSecondColor(color, 50), color, 45);
                solidBrush = new SolidColorBrush(color);
                gradientBrush.Freeze();
                solidBrush.Freeze();
                OnPropertyChanged(nameof(Color));
            }
        }

        //difference should be a maximum value of 100
        Color MakeSecondColor(Color color, uint difference)
        {
            difference = difference > 100 ? 100 : difference;
            byte r = GetNewColorByte(color.R, difference);
            byte g = GetNewColorByte(color.G, difference);
            byte b = GetNewColorByte(color.B, difference);
            return Color.FromRgb(r, g, b);
        }

        //This method ensures that bytes never overflow to avoid drastic change in color
        byte GetNewColorByte(byte oldByte, uint difference)
        {
            if (oldByte + difference > 255)
            {
                return (byte)(oldByte - difference);
            }
            else
            {
                return (byte)(oldByte + difference);
            }
        }

        public Brush GradientBrush
        {
            get { return gradientBrush; }
        }
        public Brush SolidBrush
        {
            get { return solidBrush; }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
