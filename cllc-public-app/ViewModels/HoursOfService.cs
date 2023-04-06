namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class HoursOfService
    {
        public int DayOfWeek { get; set; }
        public int? StartTimeHour { get; set; }
        public int? StartTimeMinute { get; set; }
        public int? EndTimeHour { get; set; }
        public int? EndTimeMinute { get; set; }
    }
}
