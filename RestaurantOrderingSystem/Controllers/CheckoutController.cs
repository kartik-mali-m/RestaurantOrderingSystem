using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Customer;

namespace RestaurantOrderingSystem.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;

        public CheckoutController(
            ICartService cartService,
            ApplicationDbContext context,
            IOrderService orderService)
        {
            _cartService = cartService;
            _context = context;
            _orderService = orderService;
        }


        // ==========================================
        // CHECKOUT PAGE - GET
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = _cartService.GetCart();

            // Prevent empty cart checkout
            if (cart.Items == null || !cart.Items.Any())
            {
                return RedirectToAction(
                    "Index",
                    "Cart");
            }


            // Get available tables
            var tables = await GetAvailableTablesAsync(
                cart.RestaurantId);


            // Create checkout model
            var model = new CheckoutVM
            {
                RestaurantId = cart.RestaurantId,

                RestaurantName = cart.RestaurantName,

                Items = cart.Items,

                AvailableTables = tables
            };


            return View(model);
        }


        // ==========================================
        // PLACE ORDER - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
    CheckoutVM model,
    string paymentOption)
        {
            var cart = _cartService.GetCart();

            // Prevent empty cart
            if (cart.Items == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // Restore cart data from session
            model.RestaurantId = cart.RestaurantId;
            model.RestaurantName = cart.RestaurantName;
            model.Items = cart.Items;

            // Validate payment option
            if (paymentOption != "PayNow" &&
                paymentOption != "PayLater")
            {
                ModelState.AddModelError(
                    "",
                    "Please select a payment option.");
            }

            // Validate order type
            if (model.OrderType != "DineIn" &&
                model.OrderType != "Parcel")
            {
                ModelState.AddModelError(
                    "OrderType",
                    "Please select Dine-In or Parcel.");
            }

            // Validate table for Dine-In
            if (model.OrderType == "DineIn")
            {
                if (!model.TableId.HasValue)
                {
                    ModelState.AddModelError(
                        "TableId",
                        "Please select a table.");
                }
                else
                {
                    var tableExists = await _context.RestaurantTables
                        .AnyAsync(t =>
                            t.Id == model.TableId.Value &&
                            t.RestaurantId == cart.RestaurantId &&
                            t.IsAvailable);

                    if (!tableExists)
                    {
                        ModelState.AddModelError(
                            "TableId",
                            "Selected table is no longer available.");
                    }
                }
            }

            // Validation failed
            if (!ModelState.IsValid)
            {
                model.AvailableTables =
                    await GetAvailableTablesAsync(cart.RestaurantId);

                return View(model);
            }

            // Create order
            var order = await _orderService.CreateOrderAsync(
                cart.RestaurantId,
                model.CustomerName,
                model.CustomerPhone,
                model.OrderType,
                model.TableId,
                cart);

            // Clear cart
            _cartService.ClearCart();

            // PAY NOW
            if (paymentOption == "PayNow")
            {
                return RedirectToAction(
                    "Index",
                    "Payment",
                    new { orderId = order.Id });
            }

            // PAY LATER
            return RedirectToAction(
                "Confirmation",
                "Order",
                new { id = order.Id });
        

        ModelState.AddModelError(
                "",
                "Please select a payment option.");

            model.AvailableTables =
                await GetAvailableTablesAsync(cart.RestaurantId);

            return View(model);
        }


        // ==========================================
        // GET AVAILABLE TABLES
        // ==========================================

        private async Task<List<RestaurantTableVM>>
            GetAvailableTablesAsync(int restaurantId)
        {
            return await _context.RestaurantTables
                .Where(t =>
                    t.RestaurantId == restaurantId &&
                    t.IsAvailable)
                .Select(t => new RestaurantTableVM
                {
                    TableId = t.Id,

                    TableNumber = t.TableNumber,

                    IsAvailable = t.IsAvailable
                })
                .ToListAsync();
        }
    }
}