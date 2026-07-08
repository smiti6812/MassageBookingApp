namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class MonthCalendarDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public DateOnly GridStartDate { get; set; }
        public DateOnly GridEndDate { get; set; }
        public List<MonthDayDto> Days { get; set; } = new();
    }
}
