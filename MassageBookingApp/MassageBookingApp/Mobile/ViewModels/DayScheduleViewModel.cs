using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MassageBookingApp.Mobile.Models.Calendar;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MassageBookingApp.Mobile.ViewModels;

[QueryProperty(nameof(DateQuery), "date")]
public partial class DayScheduleViewModel(ICalendarApiService calendarApiServ) : ObservableObject
{
    private readonly ICalendarApiService calendarApiService = calendarApiServ;

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

    public ObservableCollection<string> TimeSlots { get; } = new();
    public ObservableCollection<DayRoomScheduleDto> Rooms { get; } = new();

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

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
    public async Task InitializeAsync()
    {
        await LoadAsync();
    }

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

        if (cell.IsOccupied)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Booking Details",
                $"Time: {cell.TimeSlot}\nClient: {cell.ClientName}\nTherapist: {cell.TherapistName}",
                "OK");
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
}