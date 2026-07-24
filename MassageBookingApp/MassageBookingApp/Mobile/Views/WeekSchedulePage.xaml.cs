
using MassageBookingApp.Mobile.ViewModels;

namespace MassageBookingApp.Mobile.Views;

public partial class WeekSchedulePage : ContentPage
{
    private readonly WeekScheduleViewModel weekScheduleViewModel;
    public WeekSchedulePage(WeekScheduleViewModel viewModel)
    {
        InitializeComponent();
        weekScheduleViewModel = viewModel;
        BindingContext = weekScheduleViewModel;
        Loaded += WeekSchedulePage_Loaded;
    }

    private void WeekSchedulePage_Loaded(object? sender, EventArgs e) => weekScheduleViewModel.InitializeCommand.ExecuteAsync(null);

}