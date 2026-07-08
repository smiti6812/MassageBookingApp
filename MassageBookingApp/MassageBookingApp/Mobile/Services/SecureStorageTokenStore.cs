using MassageBookingApp.Mobile.Services.Interfaces;

namespace MassageBookingApp.Mobile.Services
{
    public class SecureStorageTokenStore : ITokenStore
    {
        private const string AccessTokenKey = "access_token";

        public async Task SetTokenAsync(string token)
        {
            await SecureStorage.SetAsync(AccessTokenKey, token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(AccessTokenKey);
        }

        public Task RemoveTokenAsync()
        {
            SecureStorage.Remove(AccessTokenKey);
            return Task.CompletedTask;
        }
    }
}
