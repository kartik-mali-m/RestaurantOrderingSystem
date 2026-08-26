using RestaurantOrderingSystem.Models.Base;
using RestaurantOrderingSystem.Models.Restaurant;

namespace RestaurantOrderingSystem.Models.Menu
{
    public class MenuItem : BaseEntity
    {
        public int RestaurantId { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? ImagePath { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Models.Restaurant.Restaurant Restaurant { get; set; } = null!;

        public Category Category { get; set; } = null!;
    }
}