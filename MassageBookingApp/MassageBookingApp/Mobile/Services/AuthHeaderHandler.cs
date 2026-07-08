using MassageBookingApp.Mobile.Services.Interfaces;
using System.Net.Http.Headers;

namespace MassageBookingApp.Mobile.Services
{
    public class AuthHeaderHandler(ITokenStore massageTokenStore) : DelegatingHandler
    {
        private readonly ITokenStore tokenStore = massageTokenStore;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await tokenStore.GetTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
