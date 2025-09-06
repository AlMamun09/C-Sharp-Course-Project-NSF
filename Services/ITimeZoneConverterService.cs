namespace LocalScout.Services
{
    public interface ITimeZoneConverterService
    {
        //Converts a UTC DateTime (from Firestore/DB) to the target local time (Asia/Dhaka).
        DateTime ConvertToLocalTime(DateTime utcDateTime);

        // Converts an assumed local DateTime (from user input) into UTC for storage.
        DateTime ConvertFromLocalToUtc(DateTime localDateTime);
    }
}