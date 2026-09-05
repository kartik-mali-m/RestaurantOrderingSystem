using RestaurantOrderingSystem.Models.Payment;

namespace RestaurantOrderingSystem.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int paymentId);

        Task<Payment?> GetByTransactionIdAsync(
            string transactionId);

        Task AddAsync(Payment payment);

        Task UpdateAsync(Payment payment);

        Task SaveChangesAsync();
    }
}