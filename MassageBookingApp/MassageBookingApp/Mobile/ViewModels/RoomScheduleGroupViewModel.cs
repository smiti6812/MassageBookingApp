using CommunityToolkit.Mvvm.ComponentModel;

namespace MassageBookingApp.Mobile.ViewModels
{
    public partial class RoomScheduleGroupViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isSelected;
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int RoomCapacity { get; set; }
        public List<RoomStationScheduleViewModel> Stations { get; set; } = new();
    }
}
