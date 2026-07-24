namespace MassageBookingApp.Mobile.ViewModels
{

    public partial class TherapistPagingViewModel(IList<TherapistScheduleViewModel> allItems) : PagingGenericViewModel<TherapistScheduleViewModel>(allItems, therapist => therapist.TherapistName)
    {
    }
}
