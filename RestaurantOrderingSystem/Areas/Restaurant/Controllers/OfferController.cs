using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantOrderingSystem.Models.Offer;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Offer;

namespace RestaurantOrderingSystem.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [Authorize(Roles = "RestaurantAdmin")]
    public class OfferController : Controller
    {
        private readonly IOfferService _offerService;
        private readonly IMenuService _menuService;

        public OfferController(
            IOfferService offerService,
            IMenuService menuService)
        {
            _offerService = offerService;
            _menuService = menuService;
        }

        // =====================================================
        // OFFER LIST
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var offers =
                await _offerService.GetOffersAsync(
                    restaurantId.Value);

            return View(offers);
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

            await LoadOfferDataAsync(
                restaurantId.Value);

            var model = new OfferVM
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsActive = true
            };

            return View(model);
        }

        // =====================================================
        // CREATE - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            OfferVM model)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                await LoadOfferDataAsync(
                    restaurantId.Value,
                    model.CategoryId,
                    model.MenuItemId);

                return View(model);
            }

            var result =
                await _offerService.CreateOfferAsync(
                    restaurantId.Value,
                    model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                await LoadOfferDataAsync(
                    restaurantId.Value,
                    model.CategoryId,
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
        public async Task<IActionResult> Edit(
            int id)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var offer =
                await _offerService.GetOfferAsync(
                    restaurantId.Value,
                    id);

            if (offer == null)
            {
                return NotFound();
            }

            await LoadOfferDataAsync(
                restaurantId.Value,
                offer.CategoryId,
                offer.MenuItemId);

            var model = new OfferVM
            {
                Id = offer.Id,
                Name = offer.Name,
                Description = offer.Description,
                TargetType = offer.TargetType,
                CategoryId = offer.CategoryId,
                MenuItemId = offer.MenuItemId,
                DiscountPercentage = offer.DiscountPercentage,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate,
                IsActive = offer.IsActive
            };

            return View(model);
        }

        // =====================================================
        // EDIT - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            OfferVM model)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                await LoadOfferDataAsync(
                    restaurantId.Value,
                    model.CategoryId,
                    model.MenuItemId);

                return View(model);
            }

            var result =
                await _offerService.UpdateOfferAsync(
                    restaurantId.Value,
                    model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                await LoadOfferDataAsync(
                    restaurantId.Value,
                    model.CategoryId,
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
        [Route("Restaurant/Offer/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var result =
                await _offerService.DeleteOfferAsync(
                    restaurantId.Value,
                    id);

            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Offer deactivated successfully.";

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // LOAD CATEGORIES + MENU ITEMS
        // =====================================================

        private async Task LoadOfferDataAsync(
            int restaurantId,
            int? selectedCategoryId = null,
            int? selectedMenuItemId = null)
        {
            var categories =
                await _menuService.GetCategoriesAsync(
                    restaurantId);

            var menuItems =
                await _menuService.GetMenuItemsAsync(
                    restaurantId);

            ViewBag.Categories =
                new SelectList(
                    categories,
                    "Id",
                    "Name",
                    selectedCategoryId);

            ViewBag.MenuItems =
                new SelectList(
                    menuItems,
                    "Id",
                    "Name",
                    selectedMenuItemId);

            ViewBag.TargetTypes =
                new SelectList(
                    Enum.GetValues<OfferTargetType>());
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