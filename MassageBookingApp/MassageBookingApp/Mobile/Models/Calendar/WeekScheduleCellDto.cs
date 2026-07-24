namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class WeekScheduleCellDto
    {
        public DateOnly Date { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public string BackgroundColor { get; set; } = "LightGreen";
        public Guid? BookingId { get; set; }
        public string? ClientName { get; set; }
        public Guid? TherapistId { get; set; }
        public string? TherapistName { get; set; }
        public string? BookingStartTime { get; set; }
        public string? BookingEndTime { get; set; }
        public bool IsBookingStartCell { get; set; }
        public bool IsBookingEndCell { get; set; }
    }
}
