using MassageBookingApp.Mobile.Models.Bookings;
using MassageBookingApp.Mobile.Models.Calendar;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Net.Http.Json;

namespace MassageBookingApp.Mobile.Services
{
    public class BookingApiService(HttpClient massageHttpClient) : IBookingApiService
    {
        private readonly HttpClient httpClient = massageHttpClient;

        public async Task<BookingDto> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync($"api/bookings/{bookingId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Booking load failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<BookingDto>(cancellationToken: cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Booking response was empty.");
            }

            return result;
        }

        public async Task<IReadOnlyList<AvailableTherapistDto>> GetAvailableTherapistsAsync(
            DateOnly date,
            TimeOnly startTime,
            int durationMinutes,
            Guid? ignoreBookingId = null,
            CancellationToken cancellationToken = default)
        {
            var url = $"api/calendar/available-therapists?date={date:yyyy-MM-dd}&startTime={startTime:HH\\:mm\\:ss}&durationMinutes={durationMinutes}";

            if (ignoreBookingId.HasValue)
            {
                url += $"&ignoreBookingId={ignoreBookingId.Value}";
            }

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Therapist availability load failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<List<AvailableTherapistDto>>(cancellationToken: cancellationToken);
            return result ?? new List<AvailableTherapistDto>();
        }

        public async Task<IReadOnlyList<AvailableRoomStationDto>> GetAvailableRoomStationsAsync(
            DateOnly date,
            TimeOnly startTime,
            int durationMinutes,
            Guid? ignoreBookingId = null,
            CancellationToken cancellationToken = default)
        {
            var url = $"api/calendar/available-room-stations?date={date:yyyy-MM-dd}&startTime={startTime:HH\\:mm\\:ss}&durationMinutes={durationMinutes}";

            if (ignoreBookingId.HasValue)
            {
                url += $"&ignoreBookingId={ignoreBookingId.Value}";
            }

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Room station availability load failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<List<AvailableRoomStationDto>>(cancellationToken: cancellationToken);
            return result ?? new List<AvailableRoomStationDto>();
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsJsonAsync("api/bookings", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Booking creation failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<BookingDto>(cancellationToken: cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Booking response was empty.");
            }

            return result;
        }

        public async Task<BookingDto> UpdateBookingAsync(Guid bookingId, UpdateBookingRequest request, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PutAsJsonAsync($"api/bookings/{bookingId}", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Booking update failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<BookingDto>(cancellationToken: cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Booking update response was empty.");
            }

            return result;
        }

        public async Task CancelBookingAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.DeleteAsync($"api/bookings/{bookingId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Booking cancel failed: {error}");
            }
        }
    }
}
