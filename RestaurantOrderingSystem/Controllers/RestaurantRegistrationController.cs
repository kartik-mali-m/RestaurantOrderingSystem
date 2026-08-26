using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Auth;

namespace RestaurantOrderingSystem.Controllers
{
    public class RestaurantRegistrationController : Controller
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantRegistrationController(
            IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RestaurantRegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _restaurantService.RegisterAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction("Login", "Auth");
        }
    }
}