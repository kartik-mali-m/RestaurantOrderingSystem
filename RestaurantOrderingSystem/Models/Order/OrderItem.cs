using RestaurantOrderingSystem.Models.Base;
using RestaurantOrderingSystem.Models.Menu;

namespace RestaurantOrderingSystem.Models.Order
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }

        public int MenuItemId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }


        // ==========================================
        // NAVIGATION PROPERTIES
        // ==========================================

        public Order Order { get; set; } = null!;

        public MenuItem MenuItem { get; set; } = null!;
    }
}