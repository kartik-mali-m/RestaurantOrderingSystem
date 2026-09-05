using RestaurantOrderingSystem.Constants;
using RestaurantOrderingSystem.Models.Base;
using RestaurantOrderingSystem.Models.Restaurant;
using RestaurantModel = RestaurantOrderingSystem.Models.Restaurant.Restaurant;

namespace RestaurantOrderingSystem.Models.Order
{
    public class Order : BaseEntity
    {
        public int RestaurantId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerPhone { get; set; } = string.Empty;

        public OrderType OrderType { get; set; }

        public int? TableId { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties

        // ==========================================
        // NAVIGATION PROPERTIES
        // ==========================================

        public RestaurantModel Restaurant { get; set; } = null!;

        public RestaurantTable? Table { get; set; }

        public ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();
    }
}