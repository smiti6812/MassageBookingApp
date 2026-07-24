using MassageBookingApp.Mobile.Models.Calendar;

namespace MassageBookingApp.Mobile.ViewModels
{
    public class RoomStationScheduleViewModel
    {
        public Guid RoomStationId { get; set; }
        public string RoomStationName { get; set; } = string.Empty;
        public List<DayScheduleCellDto> Cells { get; set; } = new();
    }
}
