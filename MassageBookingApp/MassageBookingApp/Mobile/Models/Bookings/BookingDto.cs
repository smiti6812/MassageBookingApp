namespace MassageBookingApp.Mobile.Models.Bookings
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhoneNumber { get; set; } = string.Empty;
        public Guid TherapistId { get; set; }
        public string TherapistName { get; set; } = string.Empty;

        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public Guid RoomStationId { get; set; }
        public string RoomStationName { get; set; } = string.Empty;
        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly MassageEndTime { get; set; }
        public TimeOnly TherapistAvailableFrom { get; set; }
        public int DurationMinutes { get; set; }
        public int BreakMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
