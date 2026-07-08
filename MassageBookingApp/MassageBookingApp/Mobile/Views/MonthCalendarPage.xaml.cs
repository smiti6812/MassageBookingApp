using MassageBookingApp.Mobile.ViewModels;

namespace MassageBookingApp.Mobile.Views;

public partial class MonthCalendarPage : ContentPage, IDisposable
{
    private readonly MonthCalendarViewModel viewModel;
    private bool disposedValue;
    public MonthCalendarPage(MonthCalendarViewModel monthCalendarViewModel)
    {
        InitializeComponent();
        BindingContext = monthCalendarViewModel;
        viewModel = monthCalendarViewModel;
        Loaded += MonthCalendarPage_Loaded;

    }

    private void MonthCalendarPage_Loaded(object? sender, EventArgs e) => viewModel.InitializeCommand.ExecuteAsync(null);

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.InitializeCommand.ExecuteAsync(null);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Loaded -= MonthCalendarPage_Loaded;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}