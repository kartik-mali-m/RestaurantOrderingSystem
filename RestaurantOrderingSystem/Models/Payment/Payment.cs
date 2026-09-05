using RestaurantOrderingSystem.Constants;
using RestaurantOrderingSystem.Models.Base;

using OrderModel = RestaurantOrderingSystem.Models.Order.Order;

namespace RestaurantOrderingSystem.Models.Payment
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string? TransactionId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public OrderModel Order { get; set; } = null!;
    }
}