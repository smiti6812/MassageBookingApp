using MassageBookingApp.Mobile.Models.Auth;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Net.Http.Json;

namespace MassageBookingApp.Mobile.Services
{
    public class AuthApiService(HttpClient massageHttpClient) : IAuthApiService
    {
        private readonly HttpClient httpClient = massageHttpClient;

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Login failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken) ?? throw new InvalidOperationException("Login response was empty.");
            return result;
        }
    }
}
