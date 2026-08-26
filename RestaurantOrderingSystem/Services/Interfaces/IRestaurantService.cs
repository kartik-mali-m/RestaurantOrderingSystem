using RestaurantOrderingSystem.Areas.Restaurant.ViewModels;
using RestaurantOrderingSystem.ViewModels.Auth;

namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<(bool Success, string Message)> RegisterAsync(
            RestaurantRegisterVM model);


        Task<ProfileVM?> GetProfileAsync(
    int restaurantId);

        Task<bool> UpdateProfileAsync(
            int restaurantId,
            ProfileVM model);


        Task<bool> ApproveAsync(int restaurantId);

        Task<List<RestaurantOrderingSystem.Models.Restaurant.Restaurant>>
            GetPendingAsync();
    }
}