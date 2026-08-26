using RestaurantOrderingSystem.Models.Base;
using Restaurant = RestaurantOrderingSystem.Models.Restaurant.Restaurant;

namespace RestaurantOrderingSystem.Models.Menu
{
    public class Category : BaseEntity
    {
        public int RestaurantId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Models.Restaurant.Restaurant Restaurant { get; set; } = null!;

        public ICollection<MenuItem> MenuItems { get; set; }
            = new List<MenuItem>();
    }
}