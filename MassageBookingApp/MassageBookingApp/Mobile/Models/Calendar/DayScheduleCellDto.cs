namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class DayScheduleCellDto
    {
        public string TimeSlot { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public bool IsBreakSlot { get; set; }
        public string BackgroundColor { get; set; } = "LightGreen";
        public Guid? BookingId { get; set; }
        public string? ClientName { get; set; }
        public string? TherapistName { get; set; }
    }
}
