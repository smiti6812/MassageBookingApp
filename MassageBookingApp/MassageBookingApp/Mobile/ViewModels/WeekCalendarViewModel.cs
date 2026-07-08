using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MassageBookingApp.Mobile.Models.Calendar;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MassageBookingApp.Mobile.ViewModels
{

    [QueryProperty(nameof(DateQuery), "date")]
    public partial class WeekCalendarViewModel(ICalendarApiService calendarApiServ) : ObservableObject
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
        private DateOnly selectedDate = DateOnly.FromDateTime(DateTime.Today);

        public ObservableCollection<WeekDaySummaryDto> Days { get; } = new();

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

                var result = await calendarApiService.GetWeekCalendarAsync(SelectedDate);

                Title = $"{result.WeekStartDate:yyyy-MM-dd} - {result.WeekEndDate:yyyy-MM-dd}";

                Days.Clear();
                foreach (var day in result.Days)
                {
                    Days.Add(day);
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
        public async Task PreviousWeekAsync()
        {
            SelectedDate = SelectedDate.AddDays(-7);
            await LoadAsync();
        }

        [RelayCommand]
        public async Task NextWeekAsync()
        {
            SelectedDate = SelectedDate.AddDays(7);
            await LoadAsync();
        }

        [RelayCommand]
        public async Task SelectDayAsync(WeekDaySummaryDto? day)
        {
            if (day == null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"//day?date={day.Date:yyyy-MM-dd}");
        }

        [RelayCommand]
        public async Task BackToMonthAsync()
        {
            await Shell.Current.GoToAsync("//month");
        }
    }
}
