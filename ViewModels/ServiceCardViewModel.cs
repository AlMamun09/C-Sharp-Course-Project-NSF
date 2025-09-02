namespace LocalScout.ViewModels
{
    public class ServiceCardViewModel
    {
        public string ServiceId { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public double Price { get; set; }
        public string PricingUnit { get; set; } = string.Empty;
        public string? PrimaryImageUrl { get; set; }
        public string ProviderBusinessName { get; set; } = string.Empty;
        public string? ProviderProfilePictureUrl { get; set; }
        public string? Location { get; set; } // Will hold the BusinessAddress
        public string? BusinessHours { get; set; }
        public DateTimeOffset JoinedDate { get; set; } // To show when the provider registered
    }
}