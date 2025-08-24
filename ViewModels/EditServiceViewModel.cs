using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NeighborhoodServiceFinder.ViewModels
{
    public class EditServiceViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string ServiceName { get; set; } = string.Empty; // To display the non-editable category name

        [Required]
        [StringLength(500, MinimumLength = 20)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100000.00)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Pricing Unit (e.g., per hour, per project)")]
        public string PricingUnit { get; set; } = string.Empty;

        [Display(Name = "Upload New Gallery Images")]
        public List<IFormFile>? NewGalleryImages { get; set; }

        // This will hold the URLs of the images already in the gallery
        public List<string> CurrentImageUrls { get; set; } = new List<string>();

        // This will hold the URLs of images the user marks for deletion.
        public List<string>? ImagesToDelete { get; set; }
    }
}