using RestaurantOrderingSystem.Models.Identity;

namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}