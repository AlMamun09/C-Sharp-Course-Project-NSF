using LocalScout.Data;
using System.Collections.Generic;

namespace LocalScout.ViewModels
{
    public class ProviderProfileViewModel
    {
        // Public Data: The provider's core profile (from ApplicationUser)
        public ApplicationUser ProviderProfile { get; set; }  = null!;

        // Public Data: The "Trust Stats" (Total Completed, Confirmed, Canceled)
        public BookingStatsViewModel Stats { get; set; }

        // Control Flag: Is the person viewing this page the owner?
        public bool IsOwner { get; set; }

        // Constructor to initialize the lists to prevent null errors in the View
        public ProviderProfileViewModel()
        {
            Stats = new BookingStatsViewModel();
        }
    }
}