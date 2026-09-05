using RestaurantOrderingSystem.Constants;
using RestaurantOrderingSystem.Models.Order;
using RestaurantOrderingSystem.Repositories.Interfaces;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Customer;

namespace RestaurantOrderingSystem.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> CreateOrderAsync(
            int restaurantId,
            string customerName,
            string customerPhone,
            string orderType,
            int? tableId,
            CartVM cart)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name is required.");

            if (string.IsNullOrWhiteSpace(customerPhone))
                throw new ArgumentException("Customer phone is required.");

            if (orderType != "DineIn" && orderType != "Parcel")
                throw new ArgumentException("Invalid order type.");

            if (orderType == "DineIn" && !tableId.HasValue)
                throw new ArgumentException(
                    "Table is required for Dine-In orders.");

            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException(
                    "Cart is empty.");

            var order = new Order
            {
                RestaurantId = restaurantId,
                CustomerName = customerName,
                CustomerPhone = customerPhone,

                OrderType = orderType == "DineIn"
                    ? OrderType.DineIn
                    : OrderType.Parcel,

                TableId = orderType == "DineIn"
                    ? tableId
                    : null,

                SubTotal = cart.Items.Sum(x =>
                    x.OriginalPrice * x.Quantity),

                DiscountAmount = cart.Items.Sum(x =>
                    (x.OriginalPrice - x.Price) * x.Quantity),

                TotalAmount = cart.TotalAmount,

                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    MenuItemId = item.MenuItemId,
                    ItemName = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                });
            }

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }
    }
}