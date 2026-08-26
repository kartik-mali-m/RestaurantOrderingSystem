using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantOrderingSystem.Areas.Restaurant.ViewModels;
using RestaurantOrderingSystem.Data;
using RestaurantOrderingSystem.Models.Identity;
using RestaurantOrderingSystem.Models.Restaurant;
using RestaurantOrderingSystem.Repositories.Interfaces;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Auth;

namespace RestaurantOrderingSystem.Services.Implementations
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<object> _passwordHasher;
        private readonly IWebHostEnvironment _environment;

        public RestaurantService(
            IRestaurantRepository restaurantRepository,
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _restaurantRepository = restaurantRepository;
            _context = context;
            _passwordHasher = new PasswordHasher<object>();
            _environment = environment;
        }

        // =========================================================
        // REGISTER RESTAURANT
        // =========================================================

        public async Task<(bool Success, string Message)> RegisterAsync(
            RestaurantRegisterVM model)
        {
            // -----------------------------------------------------
            // 1. Check whether restaurant already exists
            // -----------------------------------------------------

            var restaurantExists =
                await _restaurantRepository.GetByEmailAsync(model.Email);

            if (restaurantExists != null)
            {
                return (
                    false,
                    "A restaurant with this email already exists."
                );
            }

            // -----------------------------------------------------
            // 2. Check whether owner account already exists
            // -----------------------------------------------------

            var ownerExists = await _context.Users
                .AnyAsync(u => u.Email == model.OwnerEmail);

            if (ownerExists)
            {
                return (
                    false,
                    "An account with this owner email already exists."
                );
            }

            // -----------------------------------------------------
            // 3. Check RestaurantAdmin role
            // -----------------------------------------------------

            var restaurantAdminRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "RestaurantAdmin");

            if (restaurantAdminRole == null)
            {
                return (
                    false,
                    "RestaurantAdmin role was not found."
                );
            }

            // -----------------------------------------------------
            // 4. Validate Logo
            // -----------------------------------------------------

            if (model.Logo != null && model.Logo.Length > 0)
            {
                var allowedExtensions = new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };

                var extension = Path
                    .GetExtension(model.Logo.FileName)
                    .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    return (
                        false,
                        "Only JPG, JPEG, PNG and WEBP logo files are allowed."
                    );
                }

                if (model.Logo.Length > 2 * 1024 * 1024)
                {
                    return (
                        false,
                        "Logo size must be less than 2 MB."
                    );
                }
            }

            // -----------------------------------------------------
            // 5. Begin database transaction
            // -----------------------------------------------------

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // -------------------------------------------------
                // 6. Create Restaurant
                // -------------------------------------------------

                var restaurant = new Restaurant
                {
                    Name = model.RestaurantName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address,
                    GSTNumber = model.GSTNumber,
                    Status = "Pending"
                };

                await _restaurantRepository.CreateAsync(restaurant);

                // We need Restaurant.Id before saving the logo
                await _restaurantRepository.SaveChangesAsync();

                // -------------------------------------------------
                // 7. Save Restaurant Logo
                // -------------------------------------------------

                if (model.Logo != null && model.Logo.Length > 0)
                {
                    var extension = Path
                        .GetExtension(model.Logo.FileName)
                        .ToLowerInvariant();

                    var restaurantFolder = Path.Combine(
                        _environment.WebRootPath,
                        "images",
                        "restaurants",
                        restaurant.Id.ToString()
                    );

                    Directory.CreateDirectory(restaurantFolder);

                    var fileName = "logo" + extension;

                    var filePath = Path.Combine(
                        restaurantFolder,
                        fileName
                    );

                    using (var stream = new FileStream(
                        filePath,
                        FileMode.Create))
                    {
                        await model.Logo.CopyToAsync(stream);
                    }

                    restaurant.LogoPath =
                        $"/images/restaurants/{restaurant.Id}/{fileName}";

                    await _restaurantRepository.UpdateAsync(restaurant);

                    await _restaurantRepository.SaveChangesAsync();
                }

                // -------------------------------------------------
                // 8. Create Restaurant Admin User
                // -------------------------------------------------

                var owner = new User
                {
                    Name = model.OwnerName,
                    Email = model.OwnerEmail,
                    RoleId = restaurantAdminRole.Id,
                    RestaurantId = restaurant.Id
                };

                owner.PasswordHash =
                    _passwordHasher.HashPassword(
                        new object(),
                        model.Password
                    );

                await _context.Users.AddAsync(owner);

                await _context.SaveChangesAsync();

                // -------------------------------------------------
                // 9. Commit Transaction
                // -------------------------------------------------

                await transaction.CommitAsync();

                return (
                    true,
                    "Restaurant registration submitted successfully. Awaiting approval."
                );
            }
            catch
            {
                // -------------------------------------------------
                // 10. Rollback Database
                // -------------------------------------------------

                await transaction.RollbackAsync();

                return (
                    false,
                    "Restaurant registration failed. Please try again."
                );
            }
        }

        // =========================================================
        // APPROVE RESTAURANT
        // =========================================================

        public async Task<bool> ApproveAsync(int restaurantId)
        {
            var restaurant =
                await _restaurantRepository.GetByIdAsync(restaurantId);

            if (restaurant == null)
            {
                return false;
            }

            if (restaurant.Status == "Approved")
            {
                return true;
            }

            restaurant.Status = "Approved";

            await _restaurantRepository.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // GET PENDING RESTAURANTS
        // =========================================================

        public async Task<List<Restaurant>> GetPendingAsync()
        {
            return await _restaurantRepository.GetPendingAsync();
        }

        // =========================================================
        // GET PROFILE
        // =========================================================

        public async Task<ProfileVM?> GetProfileAsync(
            int restaurantId)
        {
            var restaurant =
                await _restaurantRepository
                    .GetByIdAsync(restaurantId);

            if (restaurant == null)
            {
                return null;
            }

            return new ProfileVM
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Email = restaurant.Email,
                Phone = restaurant.Phone,
                Address = restaurant.Address,
                GSTNumber = restaurant.GSTNumber,
                Status = restaurant.Status,
                Logo = restaurant.LogoPath
            };
        }

        // =========================================================
        // UPDATE PROFILE
        // =========================================================

        public async Task<bool> UpdateProfileAsync(
            int restaurantId,
            ProfileVM model)
        {
            var restaurant =
                await _restaurantRepository
                    .GetByIdAsync(restaurantId);

            if (restaurant == null)
            {
                return false;
            }

            // -----------------------------------------------------
            // Update editable fields
            // -----------------------------------------------------

            restaurant.Name = model.Name;
            restaurant.Phone = model.Phone;
            restaurant.Address = model.Address;

            // DO NOT UPDATE:
            // restaurant.Email
            // restaurant.GSTNumber
            // restaurant.Status

            // -----------------------------------------------------
            // Update Logo
            // -----------------------------------------------------

            if (model.LogoFile != null &&
                model.LogoFile.Length > 0)
            {
                var allowedExtensions = new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };

                var extension =
                    Path.GetExtension(model.LogoFile.FileName)
                    .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    return false;
                }

                if (model.LogoFile.Length > 2 * 1024 * 1024)
                {
                    return false;
                }

                var restaurantFolder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "restaurants",
                    restaurantId.ToString()
                );

                Directory.CreateDirectory(restaurantFolder);

                // Delete old logo
                if (!string.IsNullOrWhiteSpace(
                    restaurant.LogoPath))
                {
                    var oldLogoPath = Path.Combine(
                        _environment.WebRootPath,
                        restaurant.LogoPath
                            .TrimStart('/')
                            .Replace(
                                '/',
                                Path.DirectorySeparatorChar
                            )
                    );

                    if (File.Exists(oldLogoPath))
                    {
                        File.Delete(oldLogoPath);
                    }
                }

                var fileName = "logo" + extension;

                var filePath = Path.Combine(
                    restaurantFolder,
                    fileName
                );

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await model.LogoFile.CopyToAsync(stream);
                }

                restaurant.LogoPath =
                    $"/images/restaurants/{restaurantId}/{fileName}";
            }

            // -----------------------------------------------------
            // Save
            // -----------------------------------------------------

            await _restaurantRepository
                .UpdateAsync(restaurant);

            await _restaurantRepository
                .SaveChangesAsync();

            return true;
        }
    }
}