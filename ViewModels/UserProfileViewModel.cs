using LocalScout.Data;

namespace LocalScout.ViewModels
{
    public class UserProfileViewModel
    {
        public ApplicationUser UserProfile { get; set; } = new ApplicationUser();
        public BookingStatsViewModel Stats { get; set; } = new BookingStatsViewModel();
    }

    public class BookingStatsViewModel
    {
        public int TotalRequested { get; set; }
        public int TotalConfirmed { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalCanceled { get; set; }
    }
}