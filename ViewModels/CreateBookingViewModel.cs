using System;
using System.ComponentModel.DataAnnotations;

namespace LocalScout.ViewModels
{
    public class CreateBookingViewModel
    {
        // Hidden fields to pass along in the form
        public string ServiceId { get; set; } = string.Empty;
        public string ProviderId { get; set; } = string.Empty;

        // Information to display to the user on the booking page
        public string ServiceName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsNegotiable { get; set; }

        // Fields for the user to fill out
        [Required]
        [Display(Name = "Requested Date and Time")]
        public DateTime BookingDate { get; set; } = DateTime.Now.AddDays(1);

        [StringLength(500)]
        [Display(Name = "Notes for the Provider (optional)")]
        public string? CustomerNotes { get; set; }
    }
}