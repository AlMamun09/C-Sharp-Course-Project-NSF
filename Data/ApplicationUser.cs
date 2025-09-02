using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LocalScout.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        // --- PROPERTIES FOR SERVICE PROVIDERS ---
        [StringLength(100)]
        public string? BusinessName { get; set; }

        [StringLength(20)]
        public string? BusinessPhoneNumber { get; set; }

        public string? ProfilePictureUrl { get; set; }

        [StringLength(200)]
        public string? BusinessAddress { get; set; }

        [StringLength(500)]
        public string? BusinessDescription { get; set; }

        [StringLength(100)]
        public string? BusinessHours { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

    }
}