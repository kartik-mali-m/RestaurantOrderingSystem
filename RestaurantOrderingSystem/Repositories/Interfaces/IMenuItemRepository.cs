using RestaurantOrderingSystem.Models.Menu;

namespace RestaurantOrderingSystem.Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        Task<List<MenuItem>> GetByRestaurantIdAsync(
            int restaurantId);

        Task<List<MenuItem>> GetByCategoryAsync(
            int categoryId,
            int restaurantId);

        Task<MenuItem?> GetByIdAsync(
            int menuItemId,
            int restaurantId);

        Task AddAsync(MenuItem menuItem);

        Task UpdateAsync(MenuItem menuItem);

        Task SaveChangesAsync();
    }
}