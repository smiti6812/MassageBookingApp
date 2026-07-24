
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MassageBookingApp.Mobile.Models.Calendar;
using MassageBookingApp.Mobile.Services.Interfaces;


namespace MassageBookingApp.Mobile.ViewModels
{
    [QueryProperty(nameof(DateQuery), "date")]
    public partial class WeekScheduleViewModel : ObservableObject
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
        private DateOnly selectedDate = DateOnly.FromDateTime(DateTime.Today);

        [ObservableProperty]
        private DateOnly mondayDate;

        [ObservableProperty]
        private DateOnly tuesdayDate;

        [ObservableProperty]
        private DateOnly wednesdayDate;

        [ObservableProperty]
        private DateOnly thursdayDate;

        [ObservableProperty]
        private DateOnly fridayDate;

        [ObservableProperty]
        private DateOnly saturdayDate;

        [ObservableProperty]
        private DateOnly sundayDate;

        [ObservableProperty]
        private TherapistPagingViewModel therapistPager;

        [ObservableProperty]
        private TherapistScheduleViewModel currentTherapist;

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        [ObservableProperty]
        private string mondayHeader = string.Empty;

        [ObservableProperty]
        private string tuesdayHeader = string.Empty;

        [ObservableProperty]
        private string wednesdayHeader = string.Empty;
        [ObservableProperty]
        private string thursdayHeader = string.Empty;

        [ObservableProperty]
        private string fridayHeader = string.Empty;

        [ObservableProperty]
        private string saturdayHeader = string.Empty;

        [ObservableProperty]
        private string sundayHeader = string.Empty;

        public WeekScheduleViewModel(ICalendarApiService calendarApiServ, IBookingApiService bookingApiServ)
        {
            calendarApiService = calendarApiServ;
            bookingApiService = bookingApiServ;
            TherapistPager = new TherapistPagingViewModel([]);
            TherapistPager.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(TherapistPager.CurrentPage) ||
                    e.PropertyName == nameof(TherapistPager.SelectedPage))
                {
                    CurrentTherapist = TherapistPager.PagedItems[0];
                    OnPropertyChanged(nameof(CurrentTherapist));
                }
            };

            TherapistPager.PagedItems.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(CurrentTherapist));
            };
        }

        partial void OnErrorMessageChanged(string value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        partial void OnDateQueryChanged(string value)
        {
            if (DateOnly.TryParse(value, out var parsed))
            {
                SelectedDate = parsed;
            }
        }

        [RelayCommand]
        public async Task InitializeAsync() => await LoadAsync();

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var result = await calendarApiService.GetWeekScheduleAsync(SelectedDate);
                Title = $"Week Schedule - {result.WeekStartDate:yyyy-MM-dd} - {result.WeekEndDate:yyyy-MM-dd}";

                if (result.Days.Count >= 7)
                {
                    MondayDate = result.Days[0];
                    TuesdayDate = result.Days[1];
                    WednesdayDate = result.Days[2];
                    ThursdayDate = result.Days[3];
                    FridayDate = result.Days[4];
                    SaturdayDate = result.Days[5];
                    SundayDate = result.Days[6];

                    MondayHeader = FormatDayHeader(MondayDate, "Mon");
                    TuesdayHeader = FormatDayHeader(TuesdayDate, "Tue");
                    WednesdayHeader = FormatDayHeader(WednesdayDate, "Wed");
                    ThursdayHeader = FormatDayHeader(ThursdayDate, "Thu");
                    FridayHeader = FormatDayHeader(FridayDate, "Fri");
                    SaturdayHeader = FormatDayHeader(SaturdayDate, "Sat");
                    SundayHeader = FormatDayHeader(SundayDate, "Sun");

                }

                BuildTherapistPager(result.TherapistsRows);
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


        private void BuildTherapistPager(IEnumerable<WeeklyTherapistScheduleDto> therapists)
        {
            var groupedTherapists = therapists
          .GroupBy(r => new { r.TherapistId, r.TherapistName })
          .OrderBy(g => g.Key.TherapistName)
          .Select(group =>
          {
              var roomGroup = new TherapistScheduleViewModel
              {
                  TherapistId = group.Key.TherapistId,
                  TherapistName = group.Key.TherapistName
              };

              roomGroup.Rows.AddRange(group);
              return roomGroup;
          })
          .ToList();

            TherapistPager.UpdateAllItems(groupedTherapists);
            CurrentTherapist = TherapistPager.PagedItems.FirstOrDefault();
            OnPropertyChanged(nameof(CurrentTherapist));
        }


        private static string FormatDayHeader(DateOnly date, string dayShortName)
            => $"{dayShortName}\n{date:MM-dd}";

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
        public async Task SelectCellAsync(WeekScheduleCellDto? cell)
        {
            if (cell == null)
                return;

            if (cell.IsOccupied && cell.BookingId.HasValue)
            {
                var displayTime = !string.IsNullOrWhiteSpace(cell.BookingStartTime) && !string.IsNullOrWhiteSpace(cell.BookingEndTime)
                    ? $"{cell.BookingStartTime} - {cell.BookingEndTime}"
                    : cell.TimeSlot;

                var action = await Application.Current!.MainPage!.DisplayActionSheet(
                    $"Booking\nClient: {cell.ClientName}\nTherapist: {cell.TherapistName}\nTime: {displayTime}",
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
                    $"This slot is free on {cell.Date:yyyy-MM-dd} at {cell.TimeSlot}.\nWould you like to create a booking?",
                    "Yes",
                    "No");

                if (create)
                {
                    await Shell.Current.GoToAsync($"//booking-editor?date={cell.Date:yyyy-MM-dd}&time={cell.TimeSlot}");
                }
            }
        }

        private async Task DeleteBookingAsync(Guid bookingId)
        {
            try
            {
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

        [RelayCommand]
        public async Task BackToMonthAsync()
        {
            await Shell.Current.GoToAsync("//month");
        }

        [RelayCommand]
        public async Task BackToDayAsync()
        {
            await Shell.Current.GoToAsync($"//day?date={SelectedDate:yyyy-MM-dd}");
        }
    }
}
