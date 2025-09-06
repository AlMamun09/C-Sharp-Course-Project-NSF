namespace LocalScout.Services
{
    public class TimeZoneConverterService : ITimeZoneConverterService
    {
        private readonly TimeZoneInfo _targetTimeZone;

        public TimeZoneConverterService(IConfiguration configuration)
        {
            // Read the ID from appsettings.json
            string timeZoneId = configuration["TargetTimeZoneId"] ?? "UTC"; // Default to UTC if not found
            try
            {
                _targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                // This is a fallback in case the ID is wrong or the server OS can't find it.
                _targetTimeZone = TimeZoneInfo.Utc;
            }
        }

        public DateTime ConvertToLocalTime(DateTime utcDateTime)
        {
            // If the DateTime isn't marked as UTC, we must assume it is.
            // Firestore sometimes returns 'Unspecified' Kind from the database.
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }
            
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _targetTimeZone);
        }

        public DateTime ConvertFromLocalToUtc(DateTime localDateTime)
        {
            // When user input (like from a date picker) comes in, its Kind is 'Unspecified'.
            // We MUST assume this 'Unspecified' time IS local time in our target zone.
            if (localDateTime.Kind == DateTimeKind.Unspecified)
            {
                // This method correctly treats 'Unspecified' as local to the target zone
                // and converts it to the correct UTC timestamp for storage.
                return TimeZoneInfo.ConvertTimeToUtc(localDateTime, _targetTimeZone);
            }

            // If it was already UTC for some reason, just return it.
            if (localDateTime.Kind == DateTimeKind.Utc)
            {
                return localDateTime;
            }

            // If it's somehow marked as 'Local' (which means server local time),
            // convert from the server's local time to UTC.
            return localDateTime.ToUniversalTime();
        }
    }
}