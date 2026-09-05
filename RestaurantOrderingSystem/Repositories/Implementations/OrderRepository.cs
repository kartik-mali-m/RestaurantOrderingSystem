using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Models.Order;
using RestaurantOrderingSystem.Repositories.Interfaces;

namespace RestaurantOrderingSystem.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            //return await _context.Orders
            //    .Include(x => x.Items)
            //    .FirstOrDefaultAsync(x => x.Id == orderId);

            return await _context.Orders
    .Include(x => x.Items)
    .Include(x => x.Restaurant)
    .Include(x => x.Table)
    .FirstOrDefaultAsync(x => x.Id == orderId);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}