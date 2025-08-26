using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LocalScout.ViewModels
{
    public class CreateCategoryViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Priority Order")]
        public int PriorityOrder { get; set; }

        [Required]
        [Display(Name = "Category Image")]
        public IFormFile Image { get; set; } = null!;

        // --- ADD THIS NEW PROPERTY ---
        [Display(Name = "Is this category active?")]
        public bool IsActive { get; set; } = true;
    }
}