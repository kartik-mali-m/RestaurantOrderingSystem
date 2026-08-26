using RestaurantOrderingSystem.Models.Identity;

namespace RestaurantOrderingSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByIdAsync(int userId);

        Task<User> CreateAsync(User user);

        Task SaveChangesAsync();
    }
}