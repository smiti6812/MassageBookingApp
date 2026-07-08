namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class MonthDayDto
    {
        public DateOnly Date { get; set; }
        public int DayNumber { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public int BookingCount { get; set; }
        public int TotalMassageMinutes { get; set; }
    }
}
