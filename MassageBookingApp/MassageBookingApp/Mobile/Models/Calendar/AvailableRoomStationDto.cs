namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class AvailableRoomStationDto
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public Guid RoomStationId { get; set; }
        public string RoomStationName { get; set; } = string.Empty;
        public string DisplayText => $"{RoomName} / {RoomStationName}";
    }
}
