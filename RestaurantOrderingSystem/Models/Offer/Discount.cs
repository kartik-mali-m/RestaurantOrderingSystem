using RestaurantOrderingSystem.Models.Base;
using RestaurantOrderingSystem.Models.Menu;

namespace RestaurantOrderingSystem.Models.Offer
{
    public class Discount : BaseEntity
    {
        public int RestaurantId { get; set; }

        public int MenuItemId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal DiscountPercentage { get; set; }

        public bool IsActive { get; set; } = true;

        public Models.Restaurant.Restaurant Restaurant { get; set; }
            = null!;

        public MenuItem MenuItem { get; set; }
            = null!;
    }
}