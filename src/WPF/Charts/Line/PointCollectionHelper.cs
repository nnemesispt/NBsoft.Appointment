using System.Collections.ObjectModel;
using System.Linq;

namespace NBsoft.Appointment.WPF.Charts.Line
{
    public static class PointCollectionHelper
    {
        public static double GetTotal(this ObservableCollection<PointSegment> collection)
        {
            return collection.Sum((a) => { return a.Value; });
        }
    }
}
