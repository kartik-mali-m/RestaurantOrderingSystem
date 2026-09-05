using RestaurantOrderingSystem.Constants;
using RestaurantOrderingSystem.Models.Payment;
using RestaurantOrderingSystem.Repositories.Interfaces;
using RestaurantOrderingSystem.Services.Interfaces;

namespace RestaurantOrderingSystem.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(
            IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        // =============================================
        // GET PAYMENT BY ID
        // =============================================

        public async Task<Payment?> GetByIdAsync(
            int paymentId)
        {
            return await _paymentRepository
                .GetByIdAsync(paymentId);
        }

        // =============================================
        // GET PAYMENT BY TRANSACTION ID
        // =============================================

        public async Task<Payment?>
            GetByTransactionIdAsync(
                string transactionId)
        {
            return await _paymentRepository
                .GetByTransactionIdAsync(transactionId);
        }

        // =============================================
        // CREATE PAYMENT
        // =============================================

        public async Task<Payment> CreatePaymentAsync(
      int orderId,
      decimal amount,
      string paymentMethod)
        {
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                Status = PaymentStatus.Pending,
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return payment;
        }

        // =============================================
        // UPDATE PAYMENT STATUS
        // =============================================

        public async Task UpdatePaymentStatusAsync(
            int paymentId,
            PaymentStatus status,
            string? transactionId = null)
        {
            var payment = await _paymentRepository
                .GetByIdAsync(paymentId);

            if (payment == null)
            {
                throw new InvalidOperationException(
                    "Payment not found.");
            }

            payment.Status = status;

            if (!string.IsNullOrWhiteSpace(transactionId))
            {
                payment.TransactionId = transactionId;
            }

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();
        }
    }
}