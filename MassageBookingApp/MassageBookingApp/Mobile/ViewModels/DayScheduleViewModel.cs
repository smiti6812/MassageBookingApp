using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MassageBookingApp.Mobile.Models.Calendar;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MassageBookingApp.Mobile.ViewModels;

[QueryProperty(nameof(DateQuery), "date")]
public partial class DayScheduleViewModel : ObservableObject
{
    private readonly ICalendarApiService calendarApiService;
    private readonly IBookingApiService bookingApiService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string dateQuery = string.Empty;

    [ObservableProperty]
    private DateOnly selectedDate;

    [ObservableProperty]
    private int selectedRoomIndex;

    public ObservableCollection<string> TimeSlots { get; } = new();
    public ObservableCollection<DayRoomScheduleDto> Rooms { get; } = new();

    [ObservableProperty]
    private RoomPagingViewModel roomPager;

    [ObservableProperty]
    private RoomScheduleGroupViewModel currentRoom;
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public DayScheduleViewModel(ICalendarApiService calendarApiServ, IBookingApiService bookingApiServ)
    {
        calendarApiService = calendarApiServ;
        bookingApiService = bookingApiServ;
        RoomPager = new RoomPagingViewModel([]);
        RoomPager.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(RoomPager.CurrentPage) ||
                e.PropertyName == nameof(RoomPager.SelectedPage))
            {
                CurrentRoom = RoomPager.PagedItems[0];
                OnPropertyChanged(nameof(CurrentRoom));
            }
        };

        RoomPager.PagedItems.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(CurrentRoom));
        };
    }

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasError));
    }

    partial void OnDateQueryChanged(string value)
    {
        if (DateOnly.TryParse(value, out var parsedDate))
        {
            SelectedDate = parsedDate;
        }
    }

    partial void OnSelectedDateChanged(DateOnly value) => Title = $"Day Schedule - {value:yyyy-MM-dd}";

    [RelayCommand]
    public async Task InitializeAsync() => await LoadAsync();


    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var result = await calendarApiService.GetDayScheduleAsync(SelectedDate);

            Title = $"Day Schedule - {result.Date:yyyy-MM-dd}";

            TimeSlots.Clear();
            foreach (var slot in result.TimeSlots)
            {
                TimeSlots.Add(slot);
            }

            Rooms.Clear();
            foreach (var room in result.Rooms)
            {
                Rooms.Add(room);
            }

            BuildRoomPager(result.Rooms);
            OnPropertyChanged(nameof(CurrentRoom));
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

    private void BuildRoomPager(IEnumerable<DayRoomScheduleDto> rooms)
    {
        var groupedRooms = rooms
            .GroupBy(r => new { r.RoomId, r.RoomName, r.RoomCapacity })
            .OrderBy(g => g.Key.RoomName)
            .Select(group =>
            {
                var roomGroup = new RoomScheduleGroupViewModel
                {
                    RoomId = group.Key.RoomId,
                    RoomName = group.Key.RoomName,
                    RoomCapacity = group.Key.RoomCapacity
                };

                foreach (var station in group)
                {
                    roomGroup.Stations.Add(new RoomStationScheduleViewModel
                    {
                        RoomStationId = station.RoomStationId,
                        RoomStationName = station.RoomStationName,
                        Cells = station.Cells
                    });
                }

                return roomGroup;
            })
            .ToList();

        RoomPager.UpdateAllItems(groupedRooms);
        CurrentRoom = RoomPager.PagedItems.FirstOrDefault();
    }


    [RelayCommand]
    public async Task PreviousDayAsync()
    {
        SelectedDate = SelectedDate.AddDays(-1);
        await LoadAsync();
    }

    [RelayCommand]
    public async Task NextDayAsync()
    {
        SelectedDate = SelectedDate.AddDays(1);
        await LoadAsync();
    }

    [RelayCommand]
    public async Task BackToMonthAsync()
    {
        await Shell.Current.GoToAsync("//month");
    }

    [RelayCommand]
    public async Task BackToWeekAsync()
    {
        await Shell.Current.GoToAsync($"//week?date={SelectedDate:yyyy-MM-dd}");
    }

    [RelayCommand]
    public async Task SelectCellAsync(DayScheduleCellDto? cell)
    {
        if (cell == null)
        {
            return;
        }

        if (cell.IsOccupied && cell.BookingId.HasValue)
        {
            var action = await Application.Current!.MainPage!.DisplayActionSheet(
                $"Booking\nClient: {cell.ClientName}\nTherapist: {cell.TherapistName}\nTime: {cell.TimeSlot}",
                "Cancel",
                null,
                "Edit",
                "Delete");

            if (action == "Edit")
            {
                await Shell.Current.GoToAsync($"booking-editor?bookingId={cell.BookingId.Value}");
            }
            else if (action == "Delete")
            {
                await DeleteBookingAsync(cell.BookingId.Value);
            }
        }
        else
        {
            var create = await Application.Current!.MainPage!.DisplayAlert(
                "Free Slot",
                $"This slot is free at {cell.TimeSlot}.\nWould you like to create a booking?",
                "Yes",
                "No");

            if (create)
            {
                await OpenNewBookingForSlotAsync(cell.TimeSlot);
            }
        }
    }

    [RelayCommand]
    public async Task OpenNewBookingAsync()
    {
        await Shell.Current.GoToAsync($"//booking-editor?date={SelectedDate:yyyy-MM-dd}&time=10:00");
    }

    [RelayCommand]
    public async Task OpenNewBookingForSlotAsync(string? timeSlot)
    {
        var time = string.IsNullOrWhiteSpace(timeSlot) ? "10:00" : timeSlot;
        await Shell.Current.GoToAsync($"//booking-editor?date={SelectedDate:yyyy-MM-dd}&time={time}");
    }

    private async Task DeleteBookingAsync(Guid bookingId)
    {
        try
        {
            var bookingApiService = IPlatformApplication.Current!.Services.GetService<IBookingApiService>();

            if (bookingApiService == null)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Booking service is not available.", "OK");
                return;
            }

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Delete Booking",
                "Are you sure you want to delete this booking?",
                "Yes",
                "No");

            if (!confirm)
                return;

            await bookingApiService.CancelBookingAsync(bookingId);

            await Application.Current!.MainPage!.DisplayAlert("Deleted", "Booking deleted.", "OK");
            await LoadAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}