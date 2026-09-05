using RestaurantOrderingSystem.Constants;
using RestaurantOrderingSystem.Models.Payment;

namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment?> GetByIdAsync(int paymentId);

        Task<Payment?> GetByTransactionIdAsync(
            string transactionId);

        Task<Payment> CreatePaymentAsync(
            int orderId,
            decimal amount,
            string paymentMethod);

        Task UpdatePaymentStatusAsync(
            int paymentId,
            PaymentStatus status,
            string? transactionId = null);
    }
}
