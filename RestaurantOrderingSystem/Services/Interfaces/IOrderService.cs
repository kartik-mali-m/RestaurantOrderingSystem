using RestaurantOrderingSystem.Models.Order;
using RestaurantOrderingSystem.ViewModels.Customer;

namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(
            int restaurantId,
            string customerName,
            string customerPhone,
            string orderType,
            int? tableId,
            CartVM cart);

        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}