using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Models.Menu;
using RestaurantOrderingSystem.Repositories.Interfaces;

namespace RestaurantOrderingSystem.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetByRestaurantIdAsync(
            int restaurantId)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(
            int categoryId,
            int restaurantId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(x =>
                    x.Id == categoryId &&
                    x.RestaurantId == restaurantId);
        }

        public async Task<bool> ExistsAsync(
            int restaurantId,
            string name,
            int? excludeId = null)
        {
            var query = _context.Categories
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}