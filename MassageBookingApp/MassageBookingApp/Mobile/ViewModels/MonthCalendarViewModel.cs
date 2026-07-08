using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MassageBookingApp.Mobile.Models.Calendar;
using MassageBookingApp.Mobile.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MassageBookingApp.Mobile.ViewModels
{
    public partial class MonthCalendarViewModel : ObservableObject
    {
        private readonly ICalendarApiService calendarApiService;
        private readonly ITokenStore tokenStore;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private int year;

        [ObservableProperty]
        private int month;

        [ObservableProperty]
        private string title = string.Empty;

        public ObservableCollection<MonthDayDto> Days { get; } = new();

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public MonthCalendarViewModel(ICalendarApiService calendarApiServ, ITokenStore token)
        {
            calendarApiService = calendarApiServ;
            tokenStore = token;

            var today = DateTime.Today;
            Year = today.Year;
            Month = today.Month;
        }

        partial void OnErrorMessageChanged(string value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            var token = await tokenStore.GetTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                await Shell.Current.GoToAsync("//login");
                return;
            }

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

                var result = await calendarApiService.GetMonthCalendarAsync(Year, Month);

                Title = $"{result.MonthName} {result.Year}";
                Days.Clear();

                foreach (var day in result.Days)
                {
                    if (day.Date.Year == Year && day.Date.Month == Month)
                    {
                        Days.Add(day);
                    }
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
        public async Task PreviousMonthAsync()
        {
            var current = new DateTime(Year, Month, 1).AddMonths(-1);
            Year = current.Year;
            Month = current.Month;

            await LoadAsync();
        }

        [RelayCommand]
        public async Task NextMonthAsync()
        {
            var current = new DateTime(Year, Month, 1).AddMonths(1);
            Year = current.Year;
            Month = current.Month;

            await LoadAsync();
        }

        [RelayCommand]
        public async Task LogoutAsync()
        {
            await tokenStore.RemoveTokenAsync();
            await Shell.Current.GoToAsync("//login");
        }

        [RelayCommand]
        public async Task SelectDayAsync(MonthDayDto? day)
        {
            if (day == null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"//day?date={day.Date:yyyy-MM-dd}");
        }

        [RelayCommand]
        public async Task OpenWeekAsync(MonthDayDto? day)
        {
            if (day == null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"//week?date={day.Date:yyyy-MM-dd}");
        }
    }
}
