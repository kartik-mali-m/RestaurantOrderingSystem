using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Menu;

namespace RestaurantOrderingSystem.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [Authorize(Roles = "RestaurantAdmin")]
    public class CategoryController : Controller
    {
        private readonly IMenuService _menuService;

        public CategoryController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var categories =
                await _menuService.GetCategoriesAsync(
                    restaurantId.Value);

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var result =
                await _menuService.CreateCategoryAsync(
                    restaurantId.Value,
                    model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                return View(model);
            }

            TempData["SuccessMessage"] =
                result.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var category =
                await _menuService.GetCategoryAsync(
                    restaurantId.Value,
                    id);

            if (category == null)
            {
                return NotFound();
            }

            var model = new CategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            CategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var result =
                await _menuService.UpdateCategoryAsync(
                    restaurantId.Value,
                    model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                return View(model);
            }

            TempData["SuccessMessage"] =
                result.Message;

            return RedirectToAction(nameof(Index));
        }

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
                await _menuService.DeleteCategoryAsync(
                    restaurantId.Value,
                    id);

            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Category deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }

        private int? GetRestaurantId()
        {
            var claim =
                User.FindFirst("RestaurantId")
                ?? User.FindFirst(ClaimTypes.GroupSid);

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