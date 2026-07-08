using MassageBookingApp.Mobile.Models.Calendar;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Net.Http.Json;

namespace MassageBookingApp.Mobile.Services
{
    public class CalendarApiService(HttpClient client) : ICalendarApiService
    {
        private readonly HttpClient httpClient = client;

        public async Task<MonthCalendarDto> GetMonthCalendarAsync(int year, int month, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync($"api/calendar/month?year={year}&month={month}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Month calendar load failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<MonthCalendarDto>(cancellationToken: cancellationToken) ?? throw new InvalidOperationException("Month calendar response was empty.");
            return result;
        }

        public async Task<WeekCalendarDto> GetWeekCalendarAsync(DateOnly date, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync($"api/calendar/week?date={date:yyyy-MM-dd}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Week calendar load failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<WeekCalendarDto>(cancellationToken: cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Week calendar response was empty.");
            }

            return result;
        }

        public async Task<DayScheduleDto> GetDayScheduleAsync(DateOnly date, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync($"api/calendar/day?date={date:yyyy-MM-dd}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Day schedule load failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<DayScheduleDto>(cancellationToken: cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Day schedule response was empty.");
            }

            return result;
        }
    }
}
