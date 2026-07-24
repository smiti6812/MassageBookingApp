using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MassageBookingApp.Mobile.Helpers;
using MassageBookingApp.Mobile.Messages;
using MassageBookingApp.Mobile.Models.Bookings;
using MassageBookingApp.Mobile.Models.Calendar;
using MassageBookingApp.Mobile.Models.Clients;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MassageBookingApp.Mobile.ViewModels
{

    [QueryProperty(nameof(DateQuery), "date")]
    [QueryProperty(nameof(TimeQuery), "time")]
    [QueryProperty(nameof(BookingIdQuery), "bookingId")]
    public partial class BookingEditorViewModel : ObservableObject, IRecipient<ClientCreatedMessage>
    {
        private readonly IClientApiService clientApiService;
        private readonly IBookingApiService bookingApiService;

        private List<ClientDto> allClients = new();
        private bool suppressAutoReload;

        [ObservableProperty]
        private string dateQuery = string.Empty;

        [ObservableProperty]
        private string timeQuery = string.Empty;

        [ObservableProperty]
        private string bookingIdQuery = string.Empty;

        [ObservableProperty]
        private Guid? bookingId;

        [ObservableProperty]
        private bool isEditMode;

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

        [ObservableProperty]
        private string clientSearchText = string.Empty;

        [ObservableProperty]
        private string validationInfo = string.Empty;

        public ObservableCollection<ClientDto> Clients { get; } = new();
        public ObservableCollection<AvailableTherapistDto> AvailableTherapists { get; } = new();
        public ObservableCollection<AvailableRoomStationDto> AvailableRoomStations { get; } = new();
        public ObservableCollection<int> AllowedDurations { get; } = new();

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public bool HasValidationInfo => !string.IsNullOrWhiteSpace(ValidationInfo);
        public bool CanDelete => IsEditMode && BookingId.HasValue;

        public DateOnly BookingDate => DateOnly.FromDateTime(SelectedDate);
        public TimeOnly StartTime => TimeOnly.FromTimeSpan(SelectedStartTime);

        public BookingEditorViewModel(
            IClientApiService clientApiServ,
            IBookingApiService bookingApiServ)
        {
            clientApiService = clientApiServ;
            bookingApiService = bookingApiServ;

            WeakReferenceMessenger.Default.Register(this);
            RebuildAllowedDurations();
        }

        public void Receive(ClientCreatedMessage message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await ReloadClientsAndSelectAsync(message.Value.Id);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            });
        }

        partial void OnErrorMessageChanged(string value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        partial void OnValidationInfoChanged(string value)
        {
            OnPropertyChanged(nameof(HasValidationInfo));
        }

        partial void OnBookingIdChanged(Guid? value)
        {
            OnPropertyChanged(nameof(CanDelete));
        }

        partial void OnIsEditModeChanged(bool value)
        {
            OnPropertyChanged(nameof(CanDelete));
            Title = value ? "Edit Booking" : "New Booking";
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
                RebuildAllowedDurations();
            }
        }

        partial void OnBookingIdQueryChanged(string value)
        {
            if (Guid.TryParse(value, out var parsed))
            {
                BookingId = parsed;
                IsEditMode = true;
            }
            else
            {
                BookingId = null;
                IsEditMode = false;
            }
        }

        partial void OnSelectedStartTimeChanged(TimeSpan value)
        {
            RebuildAllowedDurations();

            if (suppressAutoReload)
            {
                return;
            }

            _ = SafeReloadAvailabilityAsync();
        }

        partial void OnSelectedDurationChanged(int value)
        {
            if (suppressAutoReload)
            {
                return;
            }

            _ = SafeReloadAvailabilityAsync();
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            if (suppressAutoReload)
            {
                return;
            }

            _ = SafeReloadAvailabilityAsync();
        }

        partial void OnClientSearchTextChanged(string value)
        {
            ApplyClientFilter();
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                await LoadClientsInternalAsync();

                if (IsEditMode && BookingId.HasValue)
                {
                    await LoadExistingBookingInternalAsync(BookingId.Value);
                }
                else
                {
                    await LoadAvailabilityInternalAsync();
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

                await LoadClientsInternalAsync();
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

        private async Task LoadClientsInternalAsync()
        {
            var clients = await clientApiService.GetClientsAsync();

            allClients = clients
                .OrderBy(x => x.FullName)
                .ToList();

            System.Diagnostics.Debug.WriteLine($"Loaded clients from API: {allClients.Count}");

            ApplyClientFilter();

            if (SelectedClient == null && Clients.Count > 0)
            {
                SelectedClient = Clients[0];
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

                await LoadAvailabilityInternalAsync();
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

        private async Task LoadAvailabilityInternalAsync()
        {
            if (!BookingRules.IsValidStartTime(StartTime))
            {
                ErrorMessage = "Selected time is outside opening hours.";
                return;
            }

            if (SelectedDuration <= 0)
            {
                ErrorMessage = "Please select a valid duration.";
                return;
            }

            var ignoreBookingId = IsEditMode ? BookingId : null;

            var therapists = await bookingApiService.GetAvailableTherapistsAsync(
                BookingDate,
                StartTime,
                SelectedDuration,
                ignoreBookingId);

            var roomStations = await bookingApiService.GetAvailableRoomStationsAsync(
                BookingDate,
                StartTime,
                SelectedDuration,
                ignoreBookingId);

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

            if (SelectedTherapist == null || !AvailableTherapists.Any(x => x.TherapistId == SelectedTherapist.TherapistId))
            {
                SelectedTherapist = AvailableTherapists.FirstOrDefault();
            }

            if (SelectedRoomStation == null || !AvailableRoomStations.Any(x => x.RoomStationId == SelectedRoomStation.RoomStationId))
            {
                SelectedRoomStation = AvailableRoomStations.FirstOrDefault();
            }
        }

        private async Task LoadExistingBookingInternalAsync(Guid bookingId)
        {
            var booking = await bookingApiService.GetBookingByIdAsync(bookingId);

            suppressAutoReload = true;

            try
            {
                SelectedDate = booking.BookingDate.ToDateTime(TimeOnly.MinValue);
                SelectedStartTime = booking.StartTime.ToTimeSpan();
                SelectedDuration = booking.DurationMinutes;
                Notes = booking.Notes ?? string.Empty;

                RebuildAllowedDurations();

                ApplyClientFilter();
                SelectedClient = allClients.FirstOrDefault(x => x.Id == booking.ClientId);

                await LoadAvailabilityInternalAsync();

                SelectedTherapist = AvailableTherapists.FirstOrDefault(x => x.TherapistId == booking.TherapistId);
                SelectedRoomStation = AvailableRoomStations.FirstOrDefault(x => x.RoomStationId == booking.RoomStationId);
            }
            finally
            {
                suppressAutoReload = false;
            }
        }

        private async Task ReloadClientsAndSelectAsync(Guid clientId)
        {
            var clients = await clientApiService.GetClientsAsync();

            allClients = clients
                .OrderBy(x => x.FullName)
                .ToList();

            ApplyClientFilter();

            SelectedClient = Clients.FirstOrDefault(x => x.Id == clientId)
                             ?? allClients.FirstOrDefault(x => x.Id == clientId);
        }

        private async Task SafeReloadAvailabilityAsync()
        {
            if (IsBusy)
            {
                return;
            }

            if (!BookingRules.IsValidStartTime(StartTime))
            {
                return;
            }

            if (!BookingRules.GetValidDurations(StartTime).Any())
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                await LoadAvailabilityInternalAsync();
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

        private void RebuildAllowedDurations()
        {
            AllowedDurations.Clear();

            var startTime = StartTime;

            if (!BookingRules.IsValidStartTime(startTime))
            {
                ValidationInfo = "Selected start time is outside opening hours (10:00 - 20:00).";
                return;
            }

            var durations = BookingRules.GetValidDurations(startTime);

            foreach (var duration in durations)
            {
                AllowedDurations.Add(duration);
            }

            if (!AllowedDurations.Contains(SelectedDuration))
            {
                SelectedDuration = AllowedDurations.FirstOrDefault();
            }

            ValidationInfo = AllowedDurations.Count == 0
                ? "No valid durations are available for the selected start time."
                : $"Valid durations for {startTime:HH\\:mm}: {string.Join(", ", AllowedDurations)} minutes";
        }

        private void ApplyClientFilter()
        {
            var selectedClientId = SelectedClient?.Id;
            var search = ClientSearchText?.Trim() ?? string.Empty;

            System.Diagnostics.Debug.WriteLine($"ApplyClientFilter called");
            System.Diagnostics.Debug.WriteLine($"Search text: '{search}'");
            System.Diagnostics.Debug.WriteLine($"All clients count: {allClients.Count}");

            var filteredList = string.IsNullOrWhiteSpace(search)
                ? allClients.OrderBy(x => x.FullName).ToList()
                : allClients
                    .Where(x =>
                        (!string.IsNullOrWhiteSpace(x.FullName) &&
                         x.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
                        ||
                        (!string.IsNullOrWhiteSpace(x.PhoneNumber) &&
                         x.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(x => x.FullName)
                    .ToList();

            Clients.Clear();

            foreach (var client in filteredList)
            {
                Clients.Add(client);
            }

            if (selectedClientId.HasValue)
            {
                SelectedClient = Clients.FirstOrDefault(x => x.Id == selectedClientId.Value);
            }

            if (SelectedClient == null && Clients.Count > 0)
            {
                SelectedClient = Clients[0];
            }

            System.Diagnostics.Debug.WriteLine($"Filtered clients count: {Clients.Count}");
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

                if (!BookingRules.IsValidStartTime(StartTime))
                {
                    ErrorMessage = "Selected start time is outside opening hours.";
                    return;
                }

                if (!AllowedDurations.Contains(SelectedDuration))
                {
                    ErrorMessage = "Selected duration is not valid for the selected start time.";
                    return;
                }

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

                if (IsEditMode && BookingId.HasValue)
                {
                    var updateRequest = new UpdateBookingRequest
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

                    var updated = await bookingApiService.UpdateBookingAsync(BookingId.Value, updateRequest);

                    await Application.Current!.MainPage!.DisplayAlert(
                        "Success",
                        $"Booking updated for {updated.ClientName}",
                        "OK");
                }
                else
                {
                    var createRequest = new CreateBookingRequest
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

                    var created = await bookingApiService.CreateBookingAsync(createRequest);

                    await Application.Current!.MainPage!.DisplayAlert(
                        "Success",
                        $"Booking created for {created.ClientName} at {created.StartTime:HH:mm}",
                        "OK");
                }

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
        public async Task DeleteAsync()
        {
            if (!IsEditMode || !BookingId.HasValue || IsBusy)
            {
                return;
            }

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Delete Booking",
                "Are you sure you want to delete this booking?",
                "Yes",
                "No");

            if (!confirm)
            {
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                await bookingApiService.CancelBookingAsync(BookingId.Value);

                await Application.Current!.MainPage!.DisplayAlert(
                    "Deleted",
                    "Booking deleted successfully.",
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