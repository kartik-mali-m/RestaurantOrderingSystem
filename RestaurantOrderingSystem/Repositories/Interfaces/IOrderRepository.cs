using RestaurantOrderingSystem.Models.Order;

namespace RestaurantOrderingSystem.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int orderId);

        Task AddAsync(Order order);

        Task UpdateAsync(Order order);

        Task SaveChangesAsync();
    }
}