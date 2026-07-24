namespace MassageBookingApp.Mobile.ViewModels
{
    public partial class RoomPagingViewModel(IList<RoomScheduleGroupViewModel> allItems) : PagingGenericViewModel<RoomScheduleGroupViewModel>(allItems, room => room.RoomName)
    {
    }
}
