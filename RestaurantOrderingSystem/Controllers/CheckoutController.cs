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

        public CheckoutController(
            ICartService cartService,
            ApplicationDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = _cartService.GetCart();

            // Prevent checkout with empty cart
            if (cart.Items == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // Get available tables for this restaurant
            var tables = await _context.RestaurantTables
                .Where(t =>
                    t.RestaurantId == cart.RestaurantId &&
                    t.IsAvailable)
                .Select(t => new RestaurantTableVM
                {
                    TableId = t.Id,
                    TableNumber = t.TableNumber,
                    IsAvailable = t.IsAvailable
                })
                .ToListAsync();

            var model = new CheckoutVM
            {
                RestaurantId = cart.RestaurantId,
                RestaurantName = cart.RestaurantName,
                TotalAmount = cart.TotalAmount,
                AvailableTables = tables
            };

            return View(model);
        }
    }
}