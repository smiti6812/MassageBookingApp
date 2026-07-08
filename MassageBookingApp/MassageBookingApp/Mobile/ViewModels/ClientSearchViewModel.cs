using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MassageBookingApp.Mobile.Models.Bookings;
using MassageBookingApp.Mobile.Models.Clients;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MassageBookingApp.Mobile.ViewModels
{
    public partial class ClientSearchViewModel(IClientApiService clientApiServ) : ObservableObject
    {
        private readonly IClientApiService clientApiService = clientApiServ;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public ObservableCollection<ClientSearchResultDto> Results { get; } = new();

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        partial void OnErrorMessageChanged(string value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        [RelayCommand]
        public async Task SearchAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                Results.Clear();

                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    return;
                }

                var results = await clientApiService.SearchClientsAsync(SearchText);

                foreach (var item in results)
                {
                    Results.Add(item);
                }
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
        public async Task OpenBookingAsync(BookingDto? booking)
        {
            if (booking == null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"//day?date={booking.BookingDate:yyyy-MM-dd}");
        }
    }
}
