using RestaurantOrderingSystem.Models.Restaurant;

namespace RestaurantOrderingSystem.Repositories.Interfaces
{
    public interface IRestaurantRepository
    {
        Task<Restaurant?> GetByIdAsync(int restaurantId);

        Task<Restaurant?> GetByEmailAsync(string email);

        Task<List<Restaurant>> GetPendingAsync();

        Task<List<Restaurant>> GetAllAsync();

        Task<Restaurant> CreateAsync(Restaurant restaurant);
        Task UpdateAsync(Restaurant restaurant);
        Task SaveChangesAsync();
    }
}