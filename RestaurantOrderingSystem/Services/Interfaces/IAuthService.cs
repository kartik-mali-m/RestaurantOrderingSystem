using RestaurantOrderingSystem.ViewModels.Auth;

namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseVM?> LoginAsync(LoginVM model);
    }
}