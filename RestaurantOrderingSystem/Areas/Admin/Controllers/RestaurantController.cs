using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingSystem.Services.Interfaces;

namespace RestaurantOrderingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

 

        // GET:
        // /Admin/Restaurant/Pending
        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var restaurants = await _restaurantService.GetPendingAsync();

            return View(restaurants);
        }


        // POST:
        // /Admin/Restaurant/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _restaurantService.ApproveAsync(id);

            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Restaurant approved successfully.";

            return RedirectToAction(nameof(Pending));
        }
    }
}