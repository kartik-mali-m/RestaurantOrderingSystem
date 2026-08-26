using RestaurantOrderingSystem.Models.Menu;

namespace RestaurantOrderingSystem.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetByRestaurantIdAsync(
            int restaurantId);

        Task<Category?> GetByIdAsync(
            int categoryId,
            int restaurantId);

        Task<bool> ExistsAsync(
            int restaurantId,
            string name,
            int? excludeId = null);

        Task AddAsync(Category category);

        Task UpdateAsync(Category category);

        Task SaveChangesAsync();
    }
}