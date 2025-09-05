using LocalScout.Models;

namespace LocalScout.ViewModels
{
    public class BookingDetailsViewModel
    {
        public Booking Booking { get; set; } = new Booking();
        public string ProviderBusinessName { get; set; } = string.Empty;
         public string ServiceName { get; set; } = string.Empty;
        public string? ServicePrimaryImageUrl { get; set; }
        public string? CustomerProfilePictureUrl { get; set; }
    }
}