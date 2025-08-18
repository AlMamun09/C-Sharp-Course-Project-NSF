using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NeighborhoodServiceFinder.ViewModels
{
    public class BecomeProviderViewModel
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

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
    }
}