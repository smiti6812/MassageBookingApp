using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MassageBookingApp.Mobile.Models.Auth;
using MassageBookingApp.Mobile.Services.Interfaces;

namespace MassageBookingApp.Mobile.ViewModels
{
    public partial class LoginViewModel(IAuthApiService authApiServ, ITokenStore token) : ObservableObject
    {
        private readonly IAuthApiService authApiService = authApiServ;
        private readonly ITokenStore tokenStore = token;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        partial void OnErrorMessageChanged(string value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Username))
                {
                    ErrorMessage = "Username is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Password is required.";
                    return;
                }

                var request = new LoginRequest
                {
                    Username = Username,
                    Password = Password
                };

                var response = await authApiService.LoginAsync(request);

                await tokenStore.SetTokenAsync(response.AccessToken);

                await Shell.Current.GoToAsync("//month");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
