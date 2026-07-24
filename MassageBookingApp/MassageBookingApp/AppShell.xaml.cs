using MassageBookingApp.Mobile.Views;

namespace MassageBookingApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("month", typeof(MonthCalendarPage));
            Routing.RegisterRoute("week", typeof(WeekCalendarPage));
            Routing.RegisterRoute("day", typeof(DaySchedulePage));
            Routing.RegisterRoute("booking-editor", typeof(BookingEditorPage));
            Routing.RegisterRoute("search-client", typeof(ClientSearchPage));
            Routing.RegisterRoute("create-client", typeof(CreateClientPage));
            Routing.RegisterRoute("week-schedule", typeof(WeekSchedulePage));
        }
    }
}
