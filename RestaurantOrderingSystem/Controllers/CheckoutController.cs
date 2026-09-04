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
        public async Task<IActionResult> Index(CheckoutVM model)
        {
            // Get current cart from session
            var cart = _cartService.GetCart();


            // ==========================================
            // PREVENT EMPTY CART
            // ==========================================

            if (cart.Items == null || !cart.Items.Any())
            {
                return RedirectToAction(
                    "Index",
                    "Cart");
            }


            // ==========================================
            // RESTORE CART DATA
            // Never trust Items from browser
            // ==========================================

            model.RestaurantId = cart.RestaurantId;

            model.RestaurantName = cart.RestaurantName;

            model.Items = cart.Items;


            // ==========================================
            // VALIDATE ORDER TYPE
            // ==========================================

            if (model.OrderType != "DineIn" &&
                model.OrderType != "Parcel")
            {
                ModelState.AddModelError(
                    "OrderType",
                    "Please select Dine-In or Parcel.");
            }


            // ==========================================
            // VALIDATE TABLE FOR DINE-IN
            // ==========================================

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
                    // Check table belongs to restaurant
                    // and is currently available
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


            // ==========================================
            // VALIDATION FAILED
            // ==========================================

            if (!ModelState.IsValid)
            {
                model.AvailableTables =
                    await GetAvailableTablesAsync(
                        cart.RestaurantId);

                return View(model);
            }


            // ==========================================
            // PHASE 12 COMPLETE CHECK
            // ==========================================

            // We are NOT creating the final order here.
            // Payment = Phase 13
            // Final Order Creation = Phase 14

            TempData["SuccessMessage"] =
                "Checkout details validated successfully!";


            // For now redirect back to Checkout
            return RedirectToAction(nameof(Index));
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