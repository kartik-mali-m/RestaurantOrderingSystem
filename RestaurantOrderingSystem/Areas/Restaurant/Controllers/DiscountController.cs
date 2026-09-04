using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Offer;

namespace RestaurantOrderingSystem.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [Authorize(Roles = "RestaurantAdmin")]
    public class DiscountController : Controller
    {
        private readonly IDiscountService _discountService;
        private readonly IMenuService _menuService;

        public DiscountController(
            IDiscountService discountService,
            IMenuService menuService)
        {
            _discountService = discountService;
            _menuService = menuService;
        }

        // =====================================================
        // DISCOUNT LIST
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var discounts =
                await _discountService.GetDiscountsAsync(
                    restaurantId.Value);

            return View(discounts);
        }

        // =====================================================
        // CREATE - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            await LoadMenuItemsAsync(
                restaurantId.Value);

            return View(new DiscountVM());
        }

        // =====================================================
        // CREATE - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            DiscountVM model)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                await LoadMenuItemsAsync(
                    restaurantId.Value,
                    model.MenuItemId);

                return View(model);
            }

            var result =
                await _discountService.CreateDiscountAsync(
                    restaurantId.Value,
                    model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                await LoadMenuItemsAsync(
                    restaurantId.Value,
                    model.MenuItemId);

                return View(model);
            }

            TempData["SuccessMessage"] =
                result.Message;

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // EDIT - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var discount =
                await _discountService.GetDiscountAsync(
                    restaurantId.Value,
                    id);

            if (discount == null)
            {
                return NotFound();
            }

            await LoadMenuItemsAsync(
                restaurantId.Value,
                discount.MenuItemId);

            var model = new DiscountVM
            {
                Id = discount.Id,
                MenuItemId = discount.MenuItemId,
                Name = discount.Name,
                DiscountPercentage =
                    discount.DiscountPercentage,
                IsActive = discount.IsActive
            };

            return View(model);
        }

        // =====================================================
        // EDIT - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            DiscountVM model)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                await LoadMenuItemsAsync(
                    restaurantId.Value,
                    model.MenuItemId);

                return View(model);
            }

            var result =
                await _discountService.UpdateDiscountAsync(
                    restaurantId.Value,
                    model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                await LoadMenuItemsAsync(
                    restaurantId.Value,
                    model.MenuItemId);

                return View(model);
            }

            TempData["SuccessMessage"] =
                result.Message;

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // DELETE / DEACTIVATE
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var result =
                await _discountService.DeleteDiscountAsync(
                    restaurantId.Value,
                    id);

            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Discount deactivated successfully.";

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // LOAD MENU ITEMS
        // =====================================================

        private async Task LoadMenuItemsAsync(
            int restaurantId,
            int? selectedMenuItemId = null)
        {
            var menuItems =
                await _menuService.GetMenuItemsAsync(
                    restaurantId);

            ViewBag.MenuItems =
                new SelectList(
                    menuItems,
                    "Id",
                    "Name",
                    selectedMenuItemId);
        }

        // =====================================================
        // RESTAURANT ID FROM JWT
        // =====================================================

        private int? GetRestaurantId()
        {
            var claim =
                User.FindFirst("RestaurantId")
                ?? User.FindFirst(
                    ClaimTypes.GroupSid);

            if (claim == null)
            {
                return null;
            }

            if (int.TryParse(
                claim.Value,
                out var restaurantId))
            {
                return restaurantId;
            }

            return null;
        }
    }
}