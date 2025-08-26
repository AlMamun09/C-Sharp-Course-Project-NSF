using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LocalScout.ViewModels
{
    public class AddNewServiceViewModel
    {
        [Required]
        [Display(Name = "Service Category")]
        public string SelectedServiceCategoryId { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 20)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100000.00)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Pricing Unit (e.g., per hour, per project)")]
        public string PricingUnit { get; set; } = string.Empty;

        [Display(Name = "Gallery Images (Optional)")]
        public List<IFormFile>? GalleryImages { get; set; }

        // This property will hold the list of categories for the dropdown
        public IEnumerable<SelectListItem> ServiceCategories { get; set; } = new List<SelectListItem>();
    }
}