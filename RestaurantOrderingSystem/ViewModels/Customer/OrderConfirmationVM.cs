using RestaurantOrderingSystem.Models.Order;

namespace RestaurantOrderingSystem.ViewModels.Customer
{
    public class OrderConfirmationVM
    {
        public int OrderId { get; set; }

        public string RestaurantName { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerPhone { get; set; } = string.Empty;

        public string OrderType { get; set; } = string.Empty;

        public string? TableNumber { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<OrderConfirmationItemVM> Items { get; set; }
            = new List<OrderConfirmationItemVM>();
    }

    public class OrderConfirmationItemVM
    {
        public string ItemName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
    }
}