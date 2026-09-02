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

        //// Add Item to Cart
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Add(int menuItemId)
        //{
        //    var menuItem = await _context.MenuItems
        //        .Include(m => m.Restaurant)
        //        .FirstOrDefaultAsync(m =>
        //            m.Id == menuItemId &&
        //            m.IsActive &&
        //            m.IsAvailable);

        //    if (menuItem == null)
        //    {
        //        return NotFound();
        //    }

        //    var cartItem = new CartItemVM
        //    {
        //        MenuItemId = menuItem.Id,
        //        Name = menuItem.Name,
        //        ImagePath = menuItem.ImagePath,
        //        Price = menuItem.Price,
        //        Quantity = 1
        //    };

        //    _cartService.AddToCart(
        //        menuItem.RestaurantId,
        //        menuItem.Restaurant.Name,
        //        cartItem);

        //    return RedirectToAction(
        //        "Restaurant",
        //        "Menu",
        //        new { restaurantId = menuItem.RestaurantId });
        //}

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

            decimal finalPrice = menuItem.Price;

            string? promotionName = null;

            string? promotionType = null;

            decimal discountPercentage = 0;

            var now = DateTime.Now;


            // ==========================================
            // STEP 1: CHECK ACTIVE OFFER
            // ==========================================

            var activeOffers = await _context.Offers
                .Where(o =>
                    o.RestaurantId == menuItem.RestaurantId &&
                    o.IsActive &&
                    o.StartDate <= now &&
                    o.EndDate >= now)
                .ToListAsync();


            var applicableOffer = activeOffers
                .Where(o =>
                    (o.TargetType == RestaurantOrderingSystem.Models.Offer.OfferTargetType.Restaurant)
                    ||
                    (o.TargetType == RestaurantOrderingSystem.Models.Offer.OfferTargetType.Category
 && o.CategoryId == menuItem.CategoryId)
                    ||
                    (o.TargetType == RestaurantOrderingSystem.Models.Offer.OfferTargetType.MenuItem
 && o.MenuItemId == menuItem.Id))
                .OrderByDescending(o => o.DiscountPercentage)
                .FirstOrDefault();


            // ==========================================
            // OFFER HAS PRIORITY
            // ==========================================

            if (applicableOffer != null)
            {
                discountPercentage =
                    applicableOffer.DiscountPercentage;

                finalPrice =
                    menuItem.Price -
                    (menuItem.Price *
                     discountPercentage / 100);

                promotionName =
                    applicableOffer.Name;

                promotionType = "Offer";
            }

            // ==========================================
            // NO OFFER → CHECK DISCOUNT
            // ==========================================

            else
            {
                //var applicableDiscount = await _context.Discounts
                //    .Where(d =>
                //        d.RestaurantId == menuItem.RestaurantId &&
                //        d.MenuItemId == menuItem.Id &&
                //        d.IsActive &&
                //        d.StartDate <= now &&
                //        d.EndDate >= now)
                //    .OrderByDescending(d => d.DiscountPercentage)
                //    .FirstOrDefaultAsync();

                var applicableDiscount = await _context.Discounts
    .Where(d =>
        d.RestaurantId == menuItem.RestaurantId &&
        d.MenuItemId == menuItem.Id &&
        d.IsActive)
    .OrderByDescending(d => d.DiscountPercentage)
    .FirstOrDefaultAsync();


                if (applicableDiscount != null)
                {
                    discountPercentage =
                        applicableDiscount.DiscountPercentage;

                    finalPrice =
                        menuItem.Price -
                        (menuItem.Price *
                         discountPercentage / 100);

                    promotionName =
                        applicableDiscount.Name;

                    promotionType = "Discount";
                }
            }


            // ==========================================
            // CREATE CART ITEM
            // ==========================================

            var cartItem = new CartItemVM
            {
                MenuItemId = menuItem.Id,

                Name = menuItem.Name,

                ImagePath = menuItem.ImagePath,

                // Original database price
                OriginalPrice = menuItem.Price,

                // Final promotional price
                Price = finalPrice,

                Quantity = 1,

                PromotionName = promotionName,

                PromotionType = promotionType,

                DiscountPercentage = discountPercentage
            };


            // ==========================================
            // ADD TO CART
            // ==========================================

            _cartService.AddToCart(
                menuItem.RestaurantId,
                menuItem.Restaurant.Name,
                cartItem);


            return RedirectToAction(
                "Restaurant",
                "Menu",
                new
                {
                    restaurantId = menuItem.RestaurantId
                });
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