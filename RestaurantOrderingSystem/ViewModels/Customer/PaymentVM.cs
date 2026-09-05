using System.ComponentModel.DataAnnotations;

namespace RestaurantOrderingSystem.ViewModels.Customer
{
    public class PaymentVM
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionId { get; set; }
    }
}