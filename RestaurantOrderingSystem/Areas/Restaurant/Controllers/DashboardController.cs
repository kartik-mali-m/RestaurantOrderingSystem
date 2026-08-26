using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RestaurantOrderingSystem.Areas.Restaurant.Controllers
{
    using global::RestaurantOrderingSystem.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace RestaurantOrderingSystem.Areas.Restaurant.Controllers
    {
        [Area("Restaurant")]
        [Authorize(Roles = "RestaurantAdmin")]
        public class DashboardController : Controller
        {
            private readonly ApplicationDbContext _context;

            public DashboardController(ApplicationDbContext context)
            {
                _context = context;
            }

            [HttpGet]
            public async Task<IActionResult> Index()
            {
                // Get RestaurantId from logged-in RestaurantAdmin JWT/Claims
                var restaurantIdClaim = User.FindFirst("RestaurantId")?.Value;

                if (string.IsNullOrEmpty(restaurantIdClaim))
                {
                    return Unauthorized();
                }

                if (!int.TryParse(restaurantIdClaim, out int restaurantId))
                {
                    return Unauthorized();
                }

                // IMPORTANT:
                // Fetch ONLY the restaurant belonging to logged-in RestaurantAdmin

                var restaurant = await _context.Restaurants
    .AsNoTracking()
    .FirstOrDefaultAsync(r => r.Id == restaurantId);

                if (restaurant == null)
                {
                    return NotFound("Restaurant not found.");
                }

                // Send database restaurant name to dashboard
                ViewBag.RestaurantName = restaurant.Name;

                return View();
            }
        }
    }
}