using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Models.Payment;
using RestaurantOrderingSystem.Repositories.Interfaces;

namespace RestaurantOrderingSystem.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =============================================
        // GET PAYMENT BY ID
        // =============================================

        public async Task<Payment?> GetByIdAsync(
            int paymentId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x =>
                    x.Id == paymentId);
        }


        // =============================================
        // GET PAYMENT BY TRANSACTION ID
        // =============================================

        public async Task<Payment?>
            GetByTransactionIdAsync(
                string transactionId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x =>
                    x.TransactionId == transactionId);
        }


        // =============================================
        // ADD PAYMENT
        // =============================================

        public async Task AddAsync(
            Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }


        // =============================================
        // UPDATE PAYMENT
        // =============================================

        public Task UpdateAsync(
            Payment payment)
        {
            _context.Payments.Update(payment);

            return Task.CompletedTask;
        }


        // =============================================
        // SAVE CHANGES
        // =============================================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}