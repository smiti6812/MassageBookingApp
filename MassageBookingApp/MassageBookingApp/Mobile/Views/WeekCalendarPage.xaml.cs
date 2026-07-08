using MassageBookingApp.Mobile.ViewModels;

namespace MassageBookingApp.Mobile.Views;

public partial class WeekCalendarPage : ContentPage
{
    private readonly WeekCalendarViewModel viewModel;
    public WeekCalendarPage(WeekCalendarViewModel weekCalendarViewModel)
    {
        InitializeComponent();
        viewModel = weekCalendarViewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.InitializeCommand.ExecuteAsync(null);
    }
}