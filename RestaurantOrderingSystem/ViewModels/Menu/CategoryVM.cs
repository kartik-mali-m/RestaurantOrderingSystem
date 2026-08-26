using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderingSystem.ViewModels.Menu
{
    public class CategoryVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}