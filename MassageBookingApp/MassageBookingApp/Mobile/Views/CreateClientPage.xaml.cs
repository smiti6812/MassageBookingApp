using MassageBookingApp.Mobile.ViewModels;

namespace MassageBookingApp.Mobile.Views;

public partial class CreateClientPage : ContentPage
{
    public CreateClientPage(CreateClientViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}