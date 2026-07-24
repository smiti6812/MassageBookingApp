using MassageBookingApp.Mobile.Models.Clients;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Net.Http.Json;

namespace MassageBookingApp.Mobile.Services
{
    public class ClientApiService(HttpClient massageHttpClient) : IClientApiService
    {
        private readonly HttpClient httpClient = massageHttpClient;

        public async Task<IReadOnlyList<ClientDto>> GetClientsAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync("api/clients", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Client load failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<List<ClientDto>>(cancellationToken: cancellationToken);

            return result ?? new List<ClientDto>();
        }

        public async Task<ClientDto> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsJsonAsync("api/clients", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Client creation failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<ClientDto>(cancellationToken: cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Client creation response was empty.");
            }

            return result;
        }

        public async Task<IReadOnlyList<ClientSearchResultDto>> SearchClientsAsync(string query, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync($"api/clients/search?query={Uri.EscapeDataString(query)}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Client search failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<List<ClientSearchResultDto>>(cancellationToken: cancellationToken);
            return result ?? new List<ClientSearchResultDto>();
        }

        public async Task<IReadOnlyList<ClientSearchResultDto>> SearchClientsWithoutBookingAsync(string query, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync($"api/clients/searchnobooking?query={Uri.EscapeDataString(query)}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Client search failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<List<ClientSearchResultDto>>(cancellationToken: cancellationToken);
            return result ?? new List<ClientSearchResultDto>();
        }
    }
}
