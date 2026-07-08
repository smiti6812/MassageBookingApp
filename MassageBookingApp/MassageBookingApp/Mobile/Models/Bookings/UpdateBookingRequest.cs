namespace MassageBookingApp.Mobile.Models.Bookings
{
    public class UpdateBookingRequest
    {
        public Guid ClientId { get; set; }
        public Guid TherapistId { get; set; }
        public Guid RoomId { get; set; }
        public Guid RoomStationId { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
    }
}
