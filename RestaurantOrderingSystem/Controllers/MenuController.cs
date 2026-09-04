//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using RestaurantOrderingSystem.Data;
//using RestaurantOrderingSystem.ViewModels.Customer;

//namespace RestaurantOrderingSystem.Controllers
//{
//    public class MenuController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public MenuController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // Public Restaurant Menu
//        // Public Restaurant Menu
//        [HttpGet]
//        [Route("Menu/Restaurant/{restaurantId:int}")]
//        public async Task<IActionResult> Restaurant(int restaurantId)
//        {
//            var restaurant = await _context.Restaurants
//                .FirstOrDefaultAsync(r =>
//                    r.Id == restaurantId &&
//                    r.Status == "Approved");

//            if (restaurant == null)
//            {
//                return NotFound();
//            }

//            var model = new RestaurantMenuVM
//            {
//                RestaurantId = restaurant.Id,
//                RestaurantName = restaurant.Name,
//                RestaurantLogo = restaurant.LogoPath,
//                Address = restaurant.Address,
//                Phone = restaurant.Phone,

//                Categories = await _context.Categories
//                    .Where(c =>
//                        c.RestaurantId == restaurantId &&
//                        c.IsActive)
//                    .Select(c => new CategoryMenuVM
//                    {
//                        CategoryId = c.Id,
//                        CategoryName = c.Name,
//                        Description = c.Description,

//                        MenuItems = _context.MenuItems
//                            .Where(m =>
//                                m.CategoryId == c.Id &&
//                                m.RestaurantId == restaurantId &&
//                                m.IsActive &&
//                                m.IsAvailable)
//                            .Select(m => new PublicMenuItemVM
//                            {
//                                MenuItemId = m.Id,
//                                Name = m.Name,
//                                Description = m.Description,
//                                Price = m.Price,
//                                ImagePath = m.ImagePath,
//                                IsAvailable = m.IsAvailable
//                            })
//                            .ToList()
//                    })
//                    .ToListAsync()
//            };

//            return View(model);
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.ViewModels.Customer;

namespace RestaurantOrderingSystem.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================
        // PUBLIC RESTAURANT MENU
        // ============================================

        [HttpGet]
        [Route("Menu/Restaurant/{restaurantId:int}")]
        public async Task<IActionResult> Restaurant(int restaurantId)
        {
            var restaurant = await _context.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.Id == restaurantId &&
                    r.Status == "Approved");

            if (restaurant == null)
            {
                return NotFound();
            }

            // ============================================
            // CURRENT DATE/TIME
            // ============================================

            var now = DateTime.Now;

            // ============================================
            // GET ACTIVE OFFERS
            // Only offers inside StartDate and EndDate
            // ============================================

            var activeOffers = await _context.Offers
                .AsNoTracking()
                .Where(o =>
                    o.RestaurantId == restaurantId &&
                    o.IsActive &&
                    o.StartDate <= now &&
                    o.EndDate >= now)
                .ToListAsync();

            // ============================================
            // GET ACTIVE DISCOUNTS
            // ============================================

            var activeDiscounts = await _context.Discounts
                .AsNoTracking()
                .Where(d =>
                    d.RestaurantId == restaurantId &&
                    d.IsActive)
                .ToListAsync();

            // ============================================
            // GET MENU DATA
            // ============================================

            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c =>
                    c.RestaurantId == restaurantId &&
                    c.IsActive)
                .Select(c => new CategoryMenuVM
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    Description = c.Description,

                    MenuItems = _context.MenuItems
                        .Where(m =>
                            m.CategoryId == c.Id &&
                            m.RestaurantId == restaurantId &&
                            m.IsActive &&
                            m.IsAvailable)
                        .Select(m => new PublicMenuItemVM
                        {
                            MenuItemId = m.Id,
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Price,
                            FinalPrice = m.Price,
                            ImagePath = m.ImagePath,
                            IsAvailable = m.IsAvailable
                        })
                        .ToList()
                })
                .ToListAsync();

            // ============================================
            // APPLY OFFER / DISCOUNT
            //
            // PRIORITY:
            // 1. OFFER
            // 2. DISCOUNT
            // 3. ORIGINAL PRICE
            // ============================================

            foreach (var category in categories)
            {
                foreach (var item in category.MenuItems)
                {
                    // ----------------------------------------
                    // FIND APPLICABLE OFFER
                    // ----------------------------------------

                    var applicableOffer = activeOffers
                        .Where(o =>
                            IsOfferApplicable(
                                o,
                                category.CategoryId,
                                item.MenuItemId))
                        .OrderByDescending(o => o.DiscountPercentage)
                        .FirstOrDefault();

                    // ========================================
                    // OFFER HAS PRIORITY
                    // ========================================

                    if (applicableOffer != null)
                    {
                        item.HasOffer = true;
                        item.HasDiscount = false;

                        item.OfferPercentage =
                            applicableOffer.DiscountPercentage;

                        item.OfferName =
                            applicableOffer.Name;

                        item.FinalPrice =
                            item.Price -
                            (item.Price *
                             applicableOffer.DiscountPercentage / 100);

                        continue;
                    }

                    // ----------------------------------------
                    // NO OFFER → CHECK NORMAL DISCOUNT
                    // ----------------------------------------

                    var applicableDiscount = activeDiscounts
                        .Where(d =>
                            d.MenuItemId == item.MenuItemId)
                        .OrderByDescending(d => d.DiscountPercentage)
                        .FirstOrDefault();

                    if (applicableDiscount != null)
                    {
                        item.HasDiscount = true;
                        item.HasOffer = false;

                        item.DiscountPercentage =
                            applicableDiscount.DiscountPercentage;

                        item.DiscountName =
                            applicableDiscount.Name;

                        item.FinalPrice =
                            item.Price -
                            (item.Price *
                             applicableDiscount.DiscountPercentage / 100);
                    }
                    else
                    {
                        item.FinalPrice = item.Price;
                    }
                }
            }

            // ============================================
            // BUILD FINAL MODEL
            // ============================================

            var model = new RestaurantMenuVM
            {
                RestaurantId = restaurant.Id,
                RestaurantName = restaurant.Name,
                RestaurantLogo = restaurant.LogoPath,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                Categories = categories
            };

            return View(model);
        }

        // ============================================
        // CHECK WHETHER OFFER APPLIES
        // ============================================

        private bool IsOfferApplicable(
            dynamic offer,
            int categoryId,
            int menuItemId)
        {
            var targetType =
                offer.TargetType.ToString();

            // Entire Restaurant
            if (targetType == "Restaurant")
            {
                return true;
            }

            // Category
            if (targetType == "Category")
            {
                return offer.CategoryId == categoryId;
            }

            // Menu Item
            if (targetType == "MenuItem")
            {
                return offer.MenuItemId == menuItemId;
            }

            return false;
        }
    }
}