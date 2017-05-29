namespace NBsoft.Appointment.WPF.Charts
{
    public interface IChart
    {
        IChartItem[] ItemList { get; set; }
        void BuildChart();
    }
}
