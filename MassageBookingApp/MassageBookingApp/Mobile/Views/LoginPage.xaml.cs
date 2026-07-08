using MassageBookingApp.Mobile.ViewModels;

namespace MassageBookingApp.Mobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}