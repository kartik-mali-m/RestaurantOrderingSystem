using RestaurantOrderingSystem.Models.Offer;

namespace RestaurantOrderingSystem.Repositories.Interfaces
{
    public interface IOfferRepository
    {
        Task<List<Offer>> GetByRestaurantIdAsync(
            int restaurantId);

        Task<Offer?> GetByIdAsync(
            int offerId,
            int restaurantId);

        Task AddAsync(Offer offer);

        Task UpdateAsync(Offer offer);

        Task SaveChangesAsync();
    }
}