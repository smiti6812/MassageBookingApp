using MassageBookingApp.Mobile.Models.Calendar;

namespace MassageBookingApp.Mobile.ViewModels
{
    public class TherapistScheduleViewModel
    {
        public Guid TherapistId { get; set; }
        public string TherapistName { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
        public List<WeeklyTherapistScheduleDto> Rows { get; set; } = new();
    }
}
