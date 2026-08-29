using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Customer;

namespace RestaurantOrderingSystem.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ApplicationDbContext _context;

        public CartController(
            ICartService cartService,
            ApplicationDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        // View Cart
        [HttpGet]
        public IActionResult Index()
        {
            var cart = _cartService.GetCart();

            return View(cart);
        }

        // Add Item to Cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int menuItemId)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.Restaurant)
                .FirstOrDefaultAsync(m =>
                    m.Id == menuItemId &&
                    m.IsActive &&
                    m.IsAvailable);

            if (menuItem == null)
            {
                return NotFound();
            }

            var cartItem = new CartItemVM
            {
                MenuItemId = menuItem.Id,
                Name = menuItem.Name,
                ImagePath = menuItem.ImagePath,
                Price = menuItem.Price,
                Quantity = 1
            };

            _cartService.AddToCart(
                menuItem.RestaurantId,
                menuItem.Restaurant.Name,
                cartItem);

            return RedirectToAction(
                "Restaurant",
                "Menu",
                new { restaurantId = menuItem.RestaurantId });
        }

        // Increase / Decrease Quantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(
            int menuItemId,
            int quantity)
        {
            _cartService.UpdateQuantity(menuItemId, quantity);

            return RedirectToAction(nameof(Index));
        }

        // Remove Item
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int menuItemId)
        {
            _cartService.RemoveFromCart(menuItemId);

            return RedirectToAction(nameof(Index));
        }

        // Clear Cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            _cartService.ClearCart();

            return RedirectToAction(nameof(Index));
        }
    }
}