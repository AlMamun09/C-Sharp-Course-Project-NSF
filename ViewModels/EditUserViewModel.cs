using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NeighborhoodServiceFinder.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Upload New Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }

        public string? CurrentProfilePictureUrl { get; set; }
    }
}