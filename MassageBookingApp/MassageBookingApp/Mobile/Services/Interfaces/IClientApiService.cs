using MassageBookingApp.Mobile.Models.Clients;

namespace MassageBookingApp.Mobile.Services.Interfaces
{
    public interface IClientApiService
    {
        Task<IReadOnlyList<ClientDto>> GetClientsAsync(CancellationToken cancellationToken = default);
        Task<ClientDto> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClientSearchResultDto>> SearchClientsAsync(string query, CancellationToken cancellationToken = default);
    }
}
