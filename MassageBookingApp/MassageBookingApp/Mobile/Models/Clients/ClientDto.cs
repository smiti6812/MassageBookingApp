namespace MassageBookingApp.Mobile.Models.Clients
{
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string DisplayText => $"{FullName} - {PhoneNumber}";
    }
}
