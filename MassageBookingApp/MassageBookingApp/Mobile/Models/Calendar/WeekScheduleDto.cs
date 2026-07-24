namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class WeekScheduleDto
    {
        public DateOnly WeekStartDate { get; set; }
        public DateOnly WeekEndDate { get; set; }
        public List<DateOnly> Days { get; set; } = new();
        public List<string> TimeSlots { get; set; } = new();
        public List<WeekScheduleRowDto> Rows { get; set; } = new();
        public List<WeeklyTherapistScheduleDto> TherapistsRows { get; set; } = new();
    }
}
