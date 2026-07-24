using MassageBookingApp.Mobile.Models.Calendar;

namespace MassageBookingApp.Mobile.Services.Interfaces
{
    public interface ICalendarApiService
    {
        Task<WeekScheduleDto> GetWeekScheduleAsync(DateOnly date, CancellationToken cancellationToken = default);
        Task<MonthCalendarDto> GetMonthCalendarAsync(int year, int month, CancellationToken cancellationToken = default);
        Task<WeekCalendarDto> GetWeekCalendarAsync(DateOnly date, CancellationToken cancellationToken = default);
        Task<DayScheduleDto> GetDayScheduleAsync(DateOnly date, CancellationToken cancellationToken = default);
    }
}
