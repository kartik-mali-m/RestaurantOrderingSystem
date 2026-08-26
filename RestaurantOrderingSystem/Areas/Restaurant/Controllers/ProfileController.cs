using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingSystem.Areas.Restaurant.ViewModels;
using RestaurantOrderingSystem.Services.Interfaces;

namespace RestaurantOrderingSystem.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [Authorize(Roles = "RestaurantAdmin")]
    public class ProfileController : Controller
    {
        private readonly IRestaurantService _restaurantService;

        public ProfileController(
            IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        // =========================================================
        // GET PROFILE
        // /Restaurant/Profile
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var restaurantIdClaim =
                User.FindFirst("RestaurantId")?.Value;

            if (string.IsNullOrEmpty(restaurantIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(
                restaurantIdClaim,
                out int restaurantId))
            {
                return Unauthorized();
            }

            var profile =
                await _restaurantService
                    .GetProfileAsync(restaurantId);

            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // =========================================================
        // POST PROFILE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileVM model)
        {
            var restaurantIdClaim =
                User.FindFirst("RestaurantId")?.Value;

            if (string.IsNullOrEmpty(restaurantIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(
                restaurantIdClaim,
                out int restaurantId))
            {
                return Unauthorized();
            }

            model.Id = restaurantId;

            if (!ModelState.IsValid)
            {
                var existingProfile =
                    await _restaurantService
                        .GetProfileAsync(restaurantId);

                if (existingProfile != null)
                {
                    model.Email = existingProfile.Email;
                    model.GSTNumber = existingProfile.GSTNumber;
                    model.Status = existingProfile.Status;
                    model.Logo = existingProfile.Logo;
                }

                return View(model);
            }

            var result =
                await _restaurantService
                    .UpdateProfileAsync(
                        restaurantId,
                        model);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update restaurant profile.");

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Restaurant profile updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}