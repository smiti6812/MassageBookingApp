namespace MassageBookingApp.Mobile.Views;

public partial class PagingView : ContentView
{
    public static readonly BindableProperty PagingViewModelProperty =
        BindableProperty.Create(
            nameof(PagingViewModel),
            typeof(object),
            typeof(PagingView),
            propertyChanged: OnPagingViewModelChanged);

    public object PagingViewModel
    {
        get => GetValue(PagingViewModelProperty);
        set => SetValue(PagingViewModelProperty, value);
    }
    public PagingView()
    {
        InitializeComponent();
    }
    private static void OnPagingViewModelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PagingView pagingView && newValue != null)
        {
            pagingView.BindingContext = newValue;
        }
    }
}