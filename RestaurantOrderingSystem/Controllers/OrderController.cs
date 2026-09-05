using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Customer;

namespace RestaurantOrderingSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderConfirmationVM
            {
                OrderId = order.Id,
                RestaurantName = order.Restaurant.Name,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,

                OrderType = order.OrderType.ToString(),

                TableNumber = order.Table?.TableNumber,

                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,

                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,

                Items = order.Items.Select(item => new OrderConfirmationItemVM
                {
                    ItemName = item.ItemName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };

            return View(model);
        }
    }
}