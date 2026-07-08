namespace MassageBookingApp.Mobile.Models.Clients
{
    public class CreateClientRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
