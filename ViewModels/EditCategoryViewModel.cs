using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LocalScout.ViewModels
{
    public class EditCategoryViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Priority Order")]
        public int PriorityOrder { get; set; }

        [Display(Name = "Is this category active?")]
        public bool IsActive { get; set; }

        [Display(Name = "Upload New Image (Optional)")]
        public IFormFile? NewImage { get; set; }

        public string? CurrentImageUrl { get; set; }
    }
}