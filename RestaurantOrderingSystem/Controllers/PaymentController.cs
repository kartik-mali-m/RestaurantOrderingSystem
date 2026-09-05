using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingSystem.Services.Interfaces;

namespace RestaurantOrderingSystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;

        public PaymentController(
            IOrderService orderService,
            IPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            var payment = await _paymentService.CreatePaymentAsync(
                order.Id,
                order.TotalAmount,
                "Online");

            // Payment gateway integration will be connected here.
            return View(payment);
        }
    }
}