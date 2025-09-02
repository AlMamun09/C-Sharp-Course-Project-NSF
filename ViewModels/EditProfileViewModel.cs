using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LocalScout.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Business Phone Number")]
        public string BusinessPhoneNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Business Address")]
        public string BusinessAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 20)]
        [Display(Name = "About Your Business (Description)")]
        public string BusinessDescription { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Business Days & Hours (e.g., Mon-Fri, 9 AM - 5 PM)")]
        public string BusinessHours { get; set; } = string.Empty;

        [Display(Name = "Upload New Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }

        public string? CurrentProfilePictureUrl { get; set; }
    }
}