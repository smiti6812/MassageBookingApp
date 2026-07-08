using MassageBookingApp.Mobile.Models.Auth;

namespace MassageBookingApp.Mobile.Services.Interfaces
{
    public interface IAuthApiService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}
