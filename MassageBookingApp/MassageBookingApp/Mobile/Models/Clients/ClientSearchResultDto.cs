using MassageBookingApp.Mobile.Models.Bookings;

namespace MassageBookingApp.Mobile.Models.Clients
{
    public class ClientSearchResultDto
    {
        public Guid ClientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<BookingDto> Bookings { get; set; } = new();
    }
}
