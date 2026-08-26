using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderingSystem.ViewModels.Menu
{
    public class MenuItemVM
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, 99999999)]
        public decimal Price { get; set; }

        public IFormFile? Image { get; set; }

        public string? ImagePath { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsActive { get; set; } = true;
    }
}