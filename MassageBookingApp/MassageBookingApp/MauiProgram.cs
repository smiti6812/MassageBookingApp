using CommunityToolkit.Maui;
using MassageBookingApp.Mobile.Helpers;
using MassageBookingApp.Mobile.Services;
using MassageBookingApp.Mobile.Services.Interfaces;
using MassageBookingApp.Mobile.ViewModels;
using MassageBookingApp.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace MassageBookingApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<ApiSettings>();

            builder.Services.AddSingleton<ITokenStore, SecureStorageTokenStore>();
            builder.Services.AddTransient<AuthHeaderHandler>();

            var authClientBuilder = builder.Services.AddHttpClient<IAuthApiService, AuthApiService>((sp, client) =>
            {
                var apiSettings = sp.GetRequiredService<ApiSettings>();
                client.BaseAddress = new Uri(apiSettings.BaseUrl);
            });

#if DEBUG
            authClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
#endif

            var calendarClientBuilder = builder.Services.AddHttpClient<ICalendarApiService, CalendarApiService>((sp, client) =>
            {
                var apiSettings = sp.GetRequiredService<ApiSettings>();
                client.BaseAddress = new Uri(apiSettings.BaseUrl);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

            var clientApiBuilder = builder.Services.AddHttpClient<IClientApiService, ClientApiService>((sp, client) =>
            {
                var apiSettings = sp.GetRequiredService<ApiSettings>();
                client.BaseAddress = new Uri(apiSettings.BaseUrl);
            })
       .AddHttpMessageHandler<AuthHeaderHandler>();

#if DEBUG
            clientApiBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
#endif

            var bookingApiBuilder = builder.Services.AddHttpClient<IBookingApiService, BookingApiService>((sp, client) =>
            {
                var apiSettings = sp.GetRequiredService<ApiSettings>();
                client.BaseAddress = new Uri(apiSettings.BaseUrl);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

#if DEBUG
            bookingApiBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
#endif

#if DEBUG
            calendarClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
#endif

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MonthCalendarViewModel>();
            builder.Services.AddTransient<WeekCalendarViewModel>();
            builder.Services.AddTransient<DayScheduleViewModel>();
            builder.Services.AddTransient<BookingEditorViewModel>();
            builder.Services.AddTransient<ClientSearchViewModel>();
            builder.Services.AddTransient<CreateClientViewModel>();
            builder.Services.AddTransient<WeekScheduleViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MonthCalendarPage>();
            builder.Services.AddTransient<WeekCalendarPage>();
            builder.Services.AddTransient<DaySchedulePage>();
            builder.Services.AddTransient<BookingEditorPage>();
            builder.Services.AddTransient<ClientSearchPage>();
            builder.Services.AddTransient<CreateClientPage>();
            builder.Services.AddTransient<WeekSchedulePage>();

            return builder.Build();
        }
    }
}
