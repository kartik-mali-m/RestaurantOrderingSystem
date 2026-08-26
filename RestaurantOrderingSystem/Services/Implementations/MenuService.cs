using RestaurantOrderingSystem.Models.Menu;
using RestaurantOrderingSystem.Repositories.Interfaces;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Menu;

namespace RestaurantOrderingSystem.Services.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IWebHostEnvironment _environment;

        public MenuService(
            ICategoryRepository categoryRepository,
            IMenuItemRepository menuItemRepository,
            IWebHostEnvironment environment)
        {
            _categoryRepository = categoryRepository;
            _menuItemRepository = menuItemRepository;
            _environment = environment;
        }

        // =========================================================
        // CATEGORY
        // =========================================================

        public async Task<List<Category>> GetCategoriesAsync(
            int restaurantId)
        {
            return await _categoryRepository
                .GetByRestaurantIdAsync(restaurantId);
        }

        public async Task<Category?> GetCategoryAsync(
            int restaurantId,
            int categoryId)
        {
            return await _categoryRepository
                .GetByIdAsync(categoryId, restaurantId);
        }

        public async Task<(bool Success, string Message)>
            CreateCategoryAsync(
                int restaurantId,
                CategoryVM model)
        {
            var name = model.Name.Trim();

            var exists =
                await _categoryRepository.ExistsAsync(
                    restaurantId,
                    name);

            if (exists)
            {
                return (
                    false,
                    "A category with this name already exists."
                );
            }

            var category = new Category
            {
                RestaurantId = restaurantId,
                Name = name,
                Description = model.Description?.Trim(),
                IsActive = true
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return (
                true,
                "Category created successfully."
            );
        }

        public async Task<(bool Success, string Message)>
            UpdateCategoryAsync(
                int restaurantId,
                CategoryVM model)
        {
            var category =
                await _categoryRepository.GetByIdAsync(
                    model.Id,
                    restaurantId);

            if (category == null)
            {
                return (
                    false,
                    "Category not found."
                );
            }

            var name = model.Name.Trim();

            var exists =
                await _categoryRepository.ExistsAsync(
                    restaurantId,
                    name,
                    model.Id);

            if (exists)
            {
                return (
                    false,
                    "A category with this name already exists."
                );
            }

            category.Name = name;
            category.Description = model.Description?.Trim();
            category.IsActive = model.IsActive;

            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return (
                true,
                "Category updated successfully."
            );
        }

        public async Task<bool> DeleteCategoryAsync(
            int restaurantId,
            int categoryId)
        {
            var category =
                await _categoryRepository.GetByIdAsync(
                    categoryId,
                    restaurantId);

            if (category == null)
            {
                return false;
            }

            // We don't physically delete the category.
            category.IsActive = false;

            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // MENU ITEMS
        // =========================================================

        public async Task<List<MenuItem>> GetMenuItemsAsync(
            int restaurantId)
        {
            return await _menuItemRepository
                .GetByRestaurantIdAsync(restaurantId);
        }

        public async Task<MenuItem?> GetMenuItemAsync(
            int restaurantId,
            int menuItemId)
        {
            return await _menuItemRepository
                .GetByIdAsync(menuItemId, restaurantId);
        }

        public async Task<(bool Success, string Message)>
            CreateMenuItemAsync(
                int restaurantId,
                MenuItemVM model)
        {
            // IMPORTANT:
            // Category must belong to THIS restaurant.
            var category =
                await _categoryRepository.GetByIdAsync(
                    model.CategoryId,
                    restaurantId);

            if (category == null)
            {
                return (
                    false,
                    "Invalid category."
                );
            }

            var menuItem = new MenuItem
            {
                RestaurantId = restaurantId,
                CategoryId = model.CategoryId,
                Name = model.Name.Trim(),
                Description = model.Description?.Trim(),
                Price = model.Price,
                IsAvailable = model.IsAvailable,
                IsActive = true
            };

            // Save image
            if (model.Image != null &&
                model.Image.Length > 0)
            {
                var imagePath = await SaveMenuImageAsync(
                    restaurantId,
                    model.Image);

                if (imagePath == null)
                {
                    return (
                        false,
                        "Invalid menu image."
                    );
                }

                menuItem.ImagePath = imagePath;
            }

            await _menuItemRepository.AddAsync(menuItem);
            await _menuItemRepository.SaveChangesAsync();

            return (
                true,
                "Menu item created successfully."
            );
        }

        public async Task<(bool Success, string Message)>
            UpdateMenuItemAsync(
                int restaurantId,
                MenuItemVM model)
        {
            var menuItem =
                await _menuItemRepository.GetByIdAsync(
                    model.Id,
                    restaurantId);

            if (menuItem == null)
            {
                return (
                    false,
                    "Menu item not found."
                );
            }

            // IMPORTANT:
            // New category must also belong to this restaurant.
            var category =
                await _categoryRepository.GetByIdAsync(
                    model.CategoryId,
                    restaurantId);

            if (category == null)
            {
                return (
                    false,
                    "Invalid category."
                );
            }

            menuItem.CategoryId = model.CategoryId;
            menuItem.Name = model.Name.Trim();
            menuItem.Description = model.Description?.Trim();
            menuItem.Price = model.Price;
            menuItem.IsAvailable = model.IsAvailable;
            menuItem.IsActive = model.IsActive;

            // Replace image only when a new image is selected.
            if (model.Image != null &&
                model.Image.Length > 0)
            {
                DeleteOldMenuImage(menuItem.ImagePath);

                var imagePath = await SaveMenuImageAsync(
                    restaurantId,
                    model.Image);

                if (imagePath == null)
                {
                    return (
                        false,
                        "Invalid menu image."
                    );
                }

                menuItem.ImagePath = imagePath;
            }

            await _menuItemRepository.UpdateAsync(menuItem);
            await _menuItemRepository.SaveChangesAsync();

            return (
                true,
                "Menu item updated successfully."
            );
        }

        public async Task<bool> DeleteMenuItemAsync(
            int restaurantId,
            int menuItemId)
        {
            var menuItem =
                await _menuItemRepository.GetByIdAsync(
                    menuItemId,
                    restaurantId);

            if (menuItem == null)
            {
                return false;
            }

            // Soft deactivate.
            menuItem.IsActive = false;
            menuItem.IsAvailable = false;

            await _menuItemRepository.UpdateAsync(menuItem);
            await _menuItemRepository.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // IMAGE
        // =========================================================

        private async Task<string?> SaveMenuImageAsync(
            int restaurantId,
            IFormFile image)
        {
            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extension =
                Path.GetExtension(image.FileName)
                .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return null;
            }

            // Maximum 2 MB
            if (image.Length > 2 * 1024 * 1024)
            {
                return null;
            }

            var restaurantFolder = Path.Combine(
                _environment.WebRootPath,
                "images",
                "restaurants",
                restaurantId.ToString(),
                "menu"
            );

            Directory.CreateDirectory(
                restaurantFolder);

            var fileName =
                $"{Guid.NewGuid():N}{extension}";

            var physicalPath = Path.Combine(
                restaurantFolder,
                fileName);

            using var stream = new FileStream(
                physicalPath,
                FileMode.Create);

            await image.CopyToAsync(stream);

            return
                $"/images/restaurants/{restaurantId}/menu/{fileName}";
        }

        private void DeleteOldMenuImage(
            string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }

            var relativePath =
                imagePath.TrimStart('/')
                    .Replace(
                        '/',
                        Path.DirectorySeparatorChar);

            var physicalPath = Path.Combine(
                _environment.WebRootPath,
                relativePath);

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}