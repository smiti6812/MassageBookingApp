using MassageBookingApp.Mobile.ViewModels;

namespace MassageBookingApp.Mobile.Views;

public partial class BookingEditorPage : ContentPage, IDisposable
{
    private readonly BookingEditorViewModel viewModel;
    private bool disposedValue;

    public BookingEditorPage(BookingEditorViewModel bookingEditorViewModel)
    {
        InitializeComponent();
        viewModel = bookingEditorViewModel;
        BindingContext = viewModel;
        Loaded += BookingEditorPage_Loaded;
    }
    private void BookingEditorPage_Loaded(object? sender, EventArgs e) => viewModel.InitializeCommand.ExecuteAsync(null);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Loaded -= BookingEditorPage_Loaded;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}