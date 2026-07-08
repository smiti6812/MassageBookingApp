namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class WeekDaySummaryDto
    {
        public DateOnly Date { get; set; }
        public string DayName { get; set; } = string.Empty;
        public bool IsToday { get; set; }
        public int BookingCount { get; set; }
        public int TotalMassageMinutes { get; set; }
    }
}
