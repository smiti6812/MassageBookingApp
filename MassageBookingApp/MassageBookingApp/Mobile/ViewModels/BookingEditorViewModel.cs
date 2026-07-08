using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MassageBookingApp.Mobile.Models.Bookings;
using MassageBookingApp.Mobile.Models.Calendar;
using MassageBookingApp.Mobile.Models.Clients;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MassageBookingApp.Mobile.ViewModels
{

    [QueryProperty(nameof(DateQuery), "date")]
    [QueryProperty(nameof(TimeQuery), "time")]
    public partial class BookingEditorViewModel(IClientApiService clientApiServ, IBookingApiService bookingApiServ) : ObservableObject
    {
        private readonly IClientApiService clientApiService = clientApiServ;
        private readonly IBookingApiService bookingApiService = bookingApiServ;

        [ObservableProperty]
        private string dateQuery = string.Empty;

        [ObservableProperty]
        private string timeQuery = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string title = "New Booking";

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private TimeSpan selectedStartTime = new(10, 0, 0);

        [ObservableProperty]
        private int selectedDuration = 60;

        [ObservableProperty]
        private ClientDto? selectedClient;

        [ObservableProperty]
        private AvailableTherapistDto? selectedTherapist;

        [ObservableProperty]
        private AvailableRoomStationDto? selectedRoomStation;

        [ObservableProperty]
        private string notes = string.Empty;

        public ObservableCollection<ClientDto> Clients { get; } = new();
        public ObservableCollection<AvailableTherapistDto> AvailableTherapists { get; } = new();
        public ObservableCollection<AvailableRoomStationDto> AvailableRoomStations { get; } = new();

        public List<int> AllowedDurations { get; } = new() { 30, 60, 120, 180 };

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public DateOnly BookingDate => DateOnly.FromDateTime(SelectedDate);
        public TimeOnly StartTime => TimeOnly.FromTimeSpan(SelectedStartTime);

        partial void OnErrorMessageChanged(string value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        partial void OnDateQueryChanged(string value)
        {
            if (DateOnly.TryParse(value, out var parsed))
            {
                SelectedDate = parsed.ToDateTime(TimeOnly.MinValue);
            }
        }

        partial void OnTimeQueryChanged(string value)
        {
            if (TimeOnly.TryParse(value, out var parsed))
            {
                SelectedStartTime = parsed.ToTimeSpan();
            }
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            await LoadClientsAsync();
            await LoadAvailabilityAsync();
        }

        [RelayCommand]
        public async Task LoadClientsAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var clients = await clientApiService.GetClientsAsync();

                Clients.Clear();
                foreach (var client in clients)
                {
                    Clients.Add(client);
                }

                SelectedClient ??= Clients.FirstOrDefault();
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
        public async Task LoadAvailabilityAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var therapists = await bookingApiService.GetAvailableTherapistsAsync(BookingDate, StartTime, SelectedDuration);
                var roomStations = await bookingApiService.GetAvailableRoomStationsAsync(BookingDate, StartTime, SelectedDuration);

                AvailableTherapists.Clear();
                foreach (var therapist in therapists)
                {
                    AvailableTherapists.Add(therapist);
                }

                AvailableRoomStations.Clear();
                foreach (var station in roomStations)
                {
                    AvailableRoomStations.Add(station);
                }

                SelectedTherapist = AvailableTherapists.FirstOrDefault();
                SelectedRoomStation = AvailableRoomStations.FirstOrDefault();
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
        public async Task OpenCreateClientAsync()
        {
            await Shell.Current.GoToAsync("create-client");
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                ErrorMessage = string.Empty;

                if (SelectedClient == null)
                {
                    ErrorMessage = "Please select a client.";
                    return;
                }

                if (SelectedTherapist == null)
                {
                    ErrorMessage = "Please select a therapist.";
                    return;
                }

                if (SelectedRoomStation == null)
                {
                    ErrorMessage = "Please select a room station.";
                    return;
                }

                IsBusy = true;

                var request = new CreateBookingRequest
                {
                    ClientId = SelectedClient.Id,
                    TherapistId = SelectedTherapist.TherapistId,
                    RoomId = SelectedRoomStation.RoomId,
                    RoomStationId = SelectedRoomStation.RoomStationId,
                    BookingDate = BookingDate,
                    StartTime = StartTime,
                    DurationMinutes = SelectedDuration,
                    Notes = Notes
                };

                var result = await bookingApiService.CreateBookingAsync(request);

                await Application.Current!.MainPage!.DisplayAlert(
                    "Success",
                    $"Booking created for {result.ClientName} at {result.StartTime:HH:mm}",
                    "OK");

                await Shell.Current.GoToAsync($"//day?date={BookingDate:yyyy-MM-dd}");
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
        public async Task CancelAsync()
        {
            await Shell.Current.GoToAsync($"//day?date={BookingDate:yyyy-MM-dd}");
        }
    }
}