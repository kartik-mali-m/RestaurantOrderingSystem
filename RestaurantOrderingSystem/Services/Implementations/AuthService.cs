using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.Repositories.Interfaces;
using RestaurantOrderingSystem.ViewModels.Auth;

namespace RestaurantOrderingSystem.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<object> _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _context = context;
            _passwordHasher = new PasswordHasher<object>();
        }

        public async Task<LoginResponseVM?> LoginAsync(LoginVM model)
        {
            // 1. Find user
            var user = await _userRepository
                .GetByEmailAsync(model.Email);

            if (user == null)
            {
                return null;
            }

            // 2. Verify password
            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    new object(),
                    user.PasswordHash,
                    model.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // =====================================================
            // 3. RESTAURANT APPROVAL CHECK
            // =====================================================

            if (user.Role.Name == "RestaurantAdmin")
            {
                // RestaurantAdmin must have a RestaurantId
                if (user.RestaurantId == null)
                {
                    return null;
                }

                // Get restaurant from database
                var restaurant = await _context.Restaurants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        r => r.Id == user.RestaurantId.Value);

                // Restaurant not found
                if (restaurant == null)
                {
                    return null;
                }

                // Restaurant must be approved
                if (restaurant.Status != "Approved")
                {
                    return null;
                }
            }

            // =====================================================
            // 4. GENERATE JWT ONLY AFTER APPROVAL
            // =====================================================

            var token = _jwtService.GenerateToken(user);

            return new LoginResponseVM
            {
                Token = token,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.Name,
                RestaurantId = user.RestaurantId
            };
        }
    }
}