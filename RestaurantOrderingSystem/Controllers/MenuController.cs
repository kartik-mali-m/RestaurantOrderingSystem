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

        // Public Restaurant Menu
        // Public Restaurant Menu
        [HttpGet]
        [Route("Menu/Restaurant/{restaurantId:int}")]
        public async Task<IActionResult> Restaurant(int restaurantId)
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r =>
                    r.Id == restaurantId &&
                    r.Status == "Approved");

            if (restaurant == null)
            {
                return NotFound();
            }

            var model = new RestaurantMenuVM
            {
                RestaurantId = restaurant.Id,
                RestaurantName = restaurant.Name,
                RestaurantLogo = restaurant.LogoPath,
                Address = restaurant.Address,
                Phone = restaurant.Phone,

                Categories = await _context.Categories
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
                                ImagePath = m.ImagePath,
                                IsAvailable = m.IsAvailable
                            })
                            .ToList()
                    })
                    .ToListAsync()
            };

            return View(model);
        }
    }
}