using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Models.Restaurant;
using RestaurantOrderingSystem.Repositories.Interfaces;

namespace RestaurantOrderingSystem.Repositories.Implementations
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly ApplicationDbContext _context;

        public RestaurantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Restaurant?> GetByIdAsync(int restaurantId)
        {
            return await _context.Restaurants
                .FirstOrDefaultAsync(r => r.Id == restaurantId);
        }

        public async Task<Restaurant?> GetByEmailAsync(string email)
        {
            return await _context.Restaurants
                .FirstOrDefaultAsync(r => r.Email == email);
        }

        public async Task<List<Restaurant>> GetPendingAsync()
        {
            return await _context.Restaurants
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.Id)
                .ToListAsync();
        }

        public async Task<List<Restaurant>> GetAllAsync()
        {
            return await _context.Restaurants
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        public async Task<Restaurant> CreateAsync(Restaurant restaurant)
        {
            await _context.Restaurants.AddAsync(restaurant);

            return restaurant;
        }

        public async Task UpdateAsync(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await Task.CompletedTask;
        }



        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}