using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Menu;

namespace RestaurantOrderingSystem.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [Authorize(Roles = "RestaurantAdmin")]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        // =====================================================
        // MENU LIST
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            var menuItems =
                await _menuService.GetMenuItemsAsync(
                    restaurantId.Value);

            return View(menuItems);
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

            await LoadCategoriesAsync(restaurantId.Value);

            return View(new MenuItemVM());
        }

        // =====================================================
        // CREATE - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [FromForm] MenuItemVM model)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            // -------------------------------------------------
            // DEBUG
            // -------------------------------------------------

            Console.WriteLine("======================================");
            Console.WriteLine("        MENU ITEM CREATE");
            Console.WriteLine("======================================");

            Console.WriteLine(
                $"RestaurantId : {restaurantId.Value}");

            Console.WriteLine(
                $"CategoryId   : {model.CategoryId}");

            Console.WriteLine(
                $"Name         : '{model.Name}'");

            Console.WriteLine(
                $"Description  : '{model.Description}'");

            Console.WriteLine(
                $"Price        : {model.Price}");

            Console.WriteLine(
                $"IsAvailable  : {model.IsAvailable}");

            Console.WriteLine(
                $"Image        : {model.Image?.FileName ?? "NO IMAGE"}");

            Console.WriteLine("======================================");


            // -------------------------------------------------
            // VALIDATION
            // -------------------------------------------------

            if (!ModelState.IsValid)
            {
                Console.WriteLine(
                    "MODEL STATE = INVALID");

                foreach (var error in ModelState)
                {
                    foreach (var message in error.Value.Errors)
                    {
                        Console.WriteLine(
                            $"FIELD: {error.Key} | ERROR: {message.ErrorMessage}");
                    }
                }

                await LoadCategoriesAsync(
                    restaurantId.Value,
                    model.CategoryId);

                return View(model);
            }


            Console.WriteLine(
                "MODEL STATE = VALID");

            Console.WriteLine(
                "Calling CreateMenuItemAsync...");


            // -------------------------------------------------
            // SERVICE
            // -------------------------------------------------

            var result =
                await _menuService.CreateMenuItemAsync(
                    restaurantId.Value,
                    model);


            Console.WriteLine(
                $"SERVICE RESULT: {result.Success}");

            Console.WriteLine(
                $"SERVICE MESSAGE: {result.Message}");


            // -------------------------------------------------
            // SERVICE FAILED
            // -------------------------------------------------

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                await LoadCategoriesAsync(
                    restaurantId.Value,
                    model.CategoryId);

                return View(model);
            }


            // -------------------------------------------------
            // SUCCESS
            // -------------------------------------------------

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

            var menuItem =
                await _menuService.GetMenuItemAsync(
                    restaurantId.Value,
                    id);

            if (menuItem == null)
            {
                return NotFound();
            }

            await LoadCategoriesAsync(
                restaurantId.Value,
                menuItem.CategoryId);

            var model = new MenuItemVM
            {
                Id = menuItem.Id,
                CategoryId = menuItem.CategoryId,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                ImagePath = menuItem.ImagePath,
                IsAvailable = menuItem.IsAvailable,
                IsActive = menuItem.IsActive
            };

            return View(model);
        }

        // =====================================================
        // EDIT - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [FromForm] MenuItemVM model)
        {
            var restaurantId = GetRestaurantId();

            if (restaurantId == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(
                    restaurantId.Value,
                    model.CategoryId);

                return View(model);
            }

            var result =
                await _menuService.UpdateMenuItemAsync(
                    restaurantId.Value,
                    model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                await LoadCategoriesAsync(
                    restaurantId.Value,
                    model.CategoryId);

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
                await _menuService.DeleteMenuItemAsync(
                    restaurantId.Value,
                    id);

            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Menu item deactivated successfully.";

            return RedirectToAction(
                nameof(Index));
        }

        // =====================================================
        // LOAD CATEGORIES
        // =====================================================

        private async Task LoadCategoriesAsync(
            int restaurantId,
            int? selectedCategoryId = null)
        {
            var categories =
                await _menuService.GetCategoriesAsync(
                    restaurantId);

            ViewBag.Categories =
                new SelectList(
                    categories,
                    "Id",
                    "Name",
                    selectedCategoryId);
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