namespace MassageBookingApp.Mobile.Helpers
{
    public class ApiSettings
    {
        // Android emulator -> localhost backend:
        // Use 10.0.2.2 for Android emulator
        // Example: https://10.0.2.2:7168/
        public string BaseUrl
        {
            get
            {
#if ANDROID
                return "https://10.0.2.2:7254/";
#elif WINDOWS
            return "https://localhost:7254/";
#else
            return "https://localhost:7254/";
#endif
            }
        }
    }
}
