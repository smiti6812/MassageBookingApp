using MassageBookingApp.Mobile.ViewModels;

namespace MassageBookingApp.Mobile.Views;

public partial class ClientSearchPage : ContentPage
{
	public ClientSearchPage(ClientSearchViewModel clientSearchViewModel)
	{
		InitializeComponent();
		BindingContext = clientSearchViewModel;
	}
}