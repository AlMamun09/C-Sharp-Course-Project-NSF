using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LocalScout.ViewModels
{
    // 1. Implement the IValidatableObject interface
    public class EditServiceViewModel : IValidatableObject
    {
        public string Id { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // 2. REMOVE validation attributes from here
        public double Price { get; set; }

        // And from here
        [Display(Name = "Pricing Unit")]
        public string? PricingUnit { get; set; }

        [Display(Name = "Price is Negotiable")]
        public bool IsNegotiable { get; set; }

        public List<string> CurrentImageUrls { get; set; } = new List<string>();

        [Display(Name = "Add New Gallery Images")]
        public List<IFormFile>? NewGalleryImages { get; set; }

        public List<string>? ImagesToDelete { get; set; }

        // 3. Add the same Validate method
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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