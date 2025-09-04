using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LocalScout.ViewModels
{
    // 1. Implement the IValidatableObject interface
    public class AddNewServiceViewModel : IValidatableObject
    {
        // Properties from your AddNewService.cshtml view
        [Display(Name = "Service Category")]
        public string SelectedServiceCategoryId { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // 2. REMOVE validation attributes from here
        public double Price { get; set; }

        // And from here
        [Display(Name = "Pricing Unit")]
        public string? PricingUnit { get; set; }

        [Display(Name = "Price is Negotiable")]
        public bool IsNegotiable { get; set; }

        [Display(Name = "Gallery Images")]
        public List<IFormFile>? GalleryImages { get; set; }

        public IEnumerable<SelectListItem> ServiceCategories { get; set; } = new List<SelectListItem>();


        // 3. Add this method to handle custom validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // This logic now controls the validation for price fields.
            // It only runs if the 'IsNegotiable' checkbox is NOT checked.
            if (!IsNegotiable)
            {
                if (string.IsNullOrWhiteSpace(PricingUnit))
                {
                    yield return new ValidationResult(
                        "The Pricing Unit (e.g., per hour, per project) field is required.",
                        new[] { nameof(PricingUnit) });
                }

                if (Price < 0.01 || Price > 100000)
                {
                    yield return new ValidationResult(
                        "The field Price must be between 0.01 and 100000.",
                        new[] { nameof(Price) });
                }
            }
        }
    }
}