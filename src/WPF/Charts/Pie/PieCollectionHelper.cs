using System.Collections.ObjectModel;
using System.Linq;

namespace NBsoft.Appointment.WPF.Charts.Pie
{
    public static class PieCollectionHelper
    {
        public static double GetTotal(this ObservableCollection<PieSegment> collection)
        {
            return collection.Sum((a) => { return a.Value; });
        }
    }
}
