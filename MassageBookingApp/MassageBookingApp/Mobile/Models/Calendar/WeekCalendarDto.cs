namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class WeekCalendarDto
    {
        public DateOnly WeekStartDate { get; set; }
        public DateOnly WeekEndDate { get; set; }
        public List<WeekDaySummaryDto> Days { get; set; } = new();
    }
}
