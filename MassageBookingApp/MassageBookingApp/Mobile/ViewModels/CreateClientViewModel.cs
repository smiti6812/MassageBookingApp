using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MassageBookingApp.Mobile.Messages;
using MassageBookingApp.Mobile.Models.Clients;
using MassageBookingApp.Mobile.Services.Interfaces;

namespace MassageBookingApp.Mobile.ViewModels
{
    public partial class CreateClientViewModel(IClientApiService clientApiServ) : ObservableObject
    {
        private readonly IClientApiService clientApiService = clientApiServ;

        [ObservableProperty]
        private string fullName = string.Empty;

        [ObservableProperty]
        private string phoneNumber = string.Empty;

        [ObservableProperty]
        private string notes = string.Empty;

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
        private async Task SaveAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(FullName))
                {
                    ErrorMessage = "Client name is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    ErrorMessage = "Phone number is required.";
                    return;
                }

                var request = new CreateClientRequest
                {
                    FullName = FullName,
                    PhoneNumber = PhoneNumber,
                    Notes = Notes
                };

                var created = await clientApiService.CreateClientAsync(request);

                WeakReferenceMessenger.Default.Send(new ClientCreatedMessage(created));

                await Application.Current!.MainPage!.DisplayAlert(
                    "Success",
                    $"Client created: {created.FullName}",
                    "OK");

                await Shell.Current.GoToAsync("..");
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

        [RelayCommand]
        private async Task CancelAsync() => await Shell.Current.GoToAsync("..");
    }
}
