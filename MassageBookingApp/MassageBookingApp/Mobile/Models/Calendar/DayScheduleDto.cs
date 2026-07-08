namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class DayScheduleDto
    {
        public DateOnly Date { get; set; }
        public List<string> TimeSlots { get; set; } = new();
        public List<DayRoomScheduleDto> Rooms { get; set; } = new();
    }
}
