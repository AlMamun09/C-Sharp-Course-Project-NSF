using System.ComponentModel.DataAnnotations;

namespace NeighborhoodServiceFinder.ViewModels
{
    public class RequestNewCategoryViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        [Display(Name = "Suggested Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 20)]
        [Display(Name = "Reason / Description for this Category")]
        public string CategoryDescription { get; set; } = string.Empty;
    }
}