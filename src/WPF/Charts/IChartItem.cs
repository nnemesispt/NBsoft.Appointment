using System.Windows.Media;

namespace NBsoft.Appointment.WPF.Charts
{
    public interface IChartItem
    {
        double Value { get; set; }
        string Name { get; set; }
        Color Color { get; set; }
    }
}
