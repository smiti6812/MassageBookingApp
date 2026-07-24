namespace MassageBookingApp.Mobile.Helpers
{
    public static class BookingRules
    {
        public static readonly TimeOnly OpeningTime = new(10, 0);
        public static readonly TimeOnly ClosingTime = new(20, 0);

        public const int TherapistBreakMinutes = 10;

        public static readonly int[] AllowedDurations = { 30, 60, 120, 150, 180 };

        public static bool IsWithinOpeningHours(TimeOnly startTime, int durationMinutes)
        {
            var massageEnd = startTime.AddMinutes(durationMinutes);
            return startTime >= OpeningTime && massageEnd <= ClosingTime;
        }

        public static List<int> GetValidDurations(TimeOnly startTime)
        {
            return AllowedDurations
                .Where(duration => IsWithinOpeningHours(startTime, duration))
                .ToList();
        }

        public static bool IsValidStartTime(TimeOnly startTime) => startTime >= OpeningTime && startTime <= ClosingTime;
    }
}
