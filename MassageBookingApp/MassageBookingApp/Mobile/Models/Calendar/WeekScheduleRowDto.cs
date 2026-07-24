namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class WeekScheduleRowDto
    {
        public string TimeSlot { get; set; } = string.Empty;
        public List<WeekScheduleCellDto> Cells { get; set; } = new();
    }
}
