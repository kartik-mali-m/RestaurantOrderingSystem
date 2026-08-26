using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Models.Menu;
using RestaurantOrderingSystem.Repositories.Interfaces;

namespace RestaurantOrderingSystem.Repositories.Implementations
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetByRestaurantIdAsync(
            int restaurantId)
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.IsActive)
                .OrderBy(x => x.Category.Name)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<List<MenuItem>> GetByCategoryAsync(
            int categoryId,
            int restaurantId)
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Where(x =>
                    x.CategoryId == categoryId &&
                    x.RestaurantId == restaurantId &&
                    x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<MenuItem?> GetByIdAsync(
            int menuItemId,
            int restaurantId)
        {
            return await _context.MenuItems
                .FirstOrDefaultAsync(x =>
                    x.Id == menuItemId &&
                    x.RestaurantId == restaurantId);
        }

        public async Task AddAsync(MenuItem menuItem)
        {
            await _context.MenuItems.AddAsync(menuItem);
        }

        public Task UpdateAsync(MenuItem menuItem)
        {
            _context.MenuItems.Update(menuItem);

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}