namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class WeeklyTherapistScheduleDto
    {
        public Guid TherapistId { get; set; }
        public string TherapistName { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
        public List<WeekScheduleCellDto> Cells { get; set; } = new();
    }
}
