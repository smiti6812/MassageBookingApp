namespace MassageBookingApp.Mobile.Models.Calendar
{
    public class DayRoomScheduleDto
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int RoomCapacity { get; set; }
        public Guid RoomStationId { get; set; }
        public string RoomStationName { get; set; } = string.Empty;
        public List<DayScheduleCellDto> Cells { get; set; } = new();
    }
}
