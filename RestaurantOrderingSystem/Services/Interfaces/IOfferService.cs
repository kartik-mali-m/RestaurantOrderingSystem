using RestaurantOrderingSystem.ViewModels.Offer;

namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface IOfferService
    {
        Task<List<Models.Offer.Offer>> GetOffersAsync(
            int restaurantId);

        Task<Models.Offer.Offer?> GetOfferAsync(
            int restaurantId,
            int offerId);

        Task<(bool Success, string Message)> CreateOfferAsync(
            int restaurantId,
            OfferVM model);

        Task<(bool Success, string Message)> UpdateOfferAsync(
            int restaurantId,
            OfferVM model);

        Task<bool> DeleteOfferAsync(
            int restaurantId,
            int offerId);
    }
}