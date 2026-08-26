using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderingSystem.Areas.Restaurant.ViewModels
{
    public class ProfileVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Restaurant Name")]
        public string Name { get; set; } = string.Empty;

        // Display only - never update this from profile
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        // Display only - never update this from profile
        public string? GSTNumber { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Logo { get; set; }

        public IFormFile? LogoFile { get; set; }
    }
}