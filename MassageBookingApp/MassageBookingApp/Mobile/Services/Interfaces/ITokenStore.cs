namespace MassageBookingApp.Mobile.Services.Interfaces
{
    public interface ITokenStore
    {
        Task SetTokenAsync(string token);
        Task<string?> GetTokenAsync();
        Task RemoveTokenAsync();
    }
}
