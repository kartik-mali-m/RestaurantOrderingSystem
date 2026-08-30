using RestaurantOrderingSystem.ViewModels.Offer;

namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<List<Models.Offer.Discount>> GetDiscountsAsync(
            int restaurantId);

        Task<Models.Offer.Discount?> GetDiscountAsync(
            int restaurantId,
            int discountId);

        Task<(bool Success, string Message)> CreateDiscountAsync(
            int restaurantId,
            DiscountVM model);

        Task<(bool Success, string Message)> UpdateDiscountAsync(
            int restaurantId,
            DiscountVM model);

        Task<bool> DeleteDiscountAsync(
            int restaurantId,
            int discountId);
    }
}