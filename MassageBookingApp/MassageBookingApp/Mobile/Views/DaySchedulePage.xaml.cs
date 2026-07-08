using MassageBookingApp.Mobile.ViewModels;

namespace MassageBookingApp.Mobile.Views;

public partial class DaySchedulePage : ContentPage, IDisposable
{
    private readonly DayScheduleViewModel viewModel;
    private bool disposedValue;
    public DaySchedulePage(DayScheduleViewModel dayScheduleViewModel)
    {
        InitializeComponent();
        viewModel = dayScheduleViewModel;
        BindingContext = viewModel;
        Loaded += DaySchedulePage_Loaded;
    }

    private void DaySchedulePage_Loaded(object? sender, EventArgs e)
    {
        if (viewModel.LoadCommand.CanExecute(null))
        {
            viewModel.LoadCommand.ExecuteAsync(null);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Loaded -= DaySchedulePage_Loaded;
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