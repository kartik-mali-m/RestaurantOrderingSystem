using RestaurantOrderingSystem.Models.Offer;

namespace RestaurantOrderingSystem.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<List<Discount>> GetByRestaurantIdAsync(
            int restaurantId);

        Task<Discount?> GetByIdAsync(
            int discountId,
            int restaurantId);

        Task<Discount?> GetActiveByMenuItemIdAsync(
            int restaurantId,
            int menuItemId);

        Task AddAsync(Discount discount);

        Task UpdateAsync(Discount discount);

        Task SaveChangesAsync();
    }
}