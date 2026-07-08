using MassageBookingApp.Mobile.Models.Bookings;
using MassageBookingApp.Mobile.Models.Calendar;

namespace MassageBookingApp.Mobile.Services.Interfaces
{
    public interface IBookingApiService
    {
        Task<IReadOnlyList<AvailableTherapistDto>> GetAvailableTherapistsAsync(
            DateOnly date,
            TimeOnly startTime,
            int durationMinutes,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AvailableRoomStationDto>> GetAvailableRoomStationsAsync(
            DateOnly date,
            TimeOnly startTime,
            int durationMinutes,
            CancellationToken cancellationToken = default);

        Task<BookingDto> CreateBookingAsync(CreateBookingRequest request, CancellationToken cancellationToken = default);
        Task<BookingDto> UpdateBookingAsync(Guid bookingId, UpdateBookingRequest request, CancellationToken cancellationToken = default);
        Task CancelBookingAsync(Guid bookingId, CancellationToken cancellationToken = default);
    }
}
