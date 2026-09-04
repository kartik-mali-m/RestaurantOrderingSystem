using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MenuService(
            ICategoryRepository categoryRepository,
            IMenuItemRepository menuItemRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _categoryRepository = categoryRepository;
            _menuItemRepository = menuItemRepository;
            _webHostEnvironment = webHostEnvironment;
        }


        // =====================================================
        // CATEGORY
        // =====================================================

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
            var categoryName = model.Name.Trim();

            var exists = await _categoryRepository.ExistsAsync(
                restaurantId,
                categoryName);

            if (exists)
            {
                return (
                    false,
                    "Category with this name already exists."
                );
            }

            var category = new Category
            {
                RestaurantId = restaurantId,
                Name = categoryName,

                Description =
                    string.IsNullOrWhiteSpace(model.Description)
                        ? null
                        : model.Description.Trim(),

                IsActive = model.IsActive
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

            var categoryName = model.Name.Trim();

            var exists =
                await _categoryRepository.ExistsAsync(
                    restaurantId,
                    categoryName,
                    model.Id);

            if (exists)
            {
                return (
                    false,
                    "Another category with this name already exists."
                );
            }

            category.Name = categoryName;

            category.Description =
                string.IsNullOrWhiteSpace(model.Description)
                    ? null
                    : model.Description.Trim();

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

            category.IsActive = false;

            await _categoryRepository.UpdateAsync(category);

            await _categoryRepository.SaveChangesAsync();

            return true;
        }


        // =====================================================
        // MENU ITEMS
        // =====================================================

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
                .GetByIdAsync(
                    menuItemId,
                    restaurantId);
        }


        // =====================================================
        // CREATE MENU ITEM
        // =====================================================

        public async Task<(bool Success, string Message)>
            CreateMenuItemAsync(
                int restaurantId,
                MenuItemVM model)
        {
            // =============================================
            // CHECK CATEGORY
            // =============================================

            var category =
                await _categoryRepository.GetByIdAsync(
                    model.CategoryId,
                    restaurantId);

            if (category == null)
            {
                return (
                    false,
                    "Selected category is invalid."
                );
            }


            // =============================================
            // SAVE IMAGE
            // =============================================

            string? imagePath = null;

            if (model.Image != null &&
                model.Image.Length > 0)
            {
                var allowedExtensions = new[]
                {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

                var extension =
                    Path.GetExtension(model.Image.FileName)
                        .ToLowerInvariant();

                // Check extension
                if (!allowedExtensions.Contains(extension))
                {
                    return (
                        false,
                        "Only JPG, JPEG, PNG and WEBP images are allowed."
                    );
                }

                // Maximum 5 MB
                if (model.Image.Length > 5 * 1024 * 1024)
                {
                    return (
                        false,
                        "Image size cannot be greater than 5 MB."
                    );
                }


                // =============================================
                // FOLDER PATH
                // wwwroot/images/menuitem
                // =============================================

                var uploadsFolder =
                    Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images",
                        "menuitem");


                // Create folder if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                // =============================================
                // CREATE UNIQUE FILE NAME
                // =============================================

                var fileName =
                    $"{Guid.NewGuid()}{extension}";


                var filePath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);


                // =============================================
                // SAVE IMAGE TO FOLDER
                // =============================================

                using (var stream =
                       new FileStream(
                           filePath,
                                             FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }


                // =============================================
                // PATH THAT WILL BE SAVED IN DATABASE
                // =============================================

                imagePath =
                    $"/images/menuitem/{fileName}";
            }


            // =============================================
            // CREATE MENU ITEM
            // =============================================

            var menuItem = new MenuItem
            {
                RestaurantId = restaurantId,

                CategoryId = model.CategoryId,

                Name = model.Name.Trim(),

                Description =
                    string.IsNullOrWhiteSpace(model.Description)
                        ? null
                        : model.Description.Trim(),

                Price = model.Price,

                ImagePath = imagePath,

                IsAvailable = model.IsAvailable,

                IsActive = model.IsActive
            };


            // =============================================
            // SAVE TO DATABASE
            // =============================================

            await _menuItemRepository.AddAsync(menuItem);

            await _menuItemRepository.SaveChangesAsync();


            return (
                true,
                "Menu item created successfully."
            );
        }
        // =====================================================
        // UPDATE MENU ITEM
        // =====================================================

        public async Task<(bool Success, string Message)>
            UpdateMenuItemAsync(
                int restaurantId,
                MenuItemVM model)
        {
            // Get existing menu item
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


            // Check category belongs to restaurant
            var category =
                await _categoryRepository.GetByIdAsync(
                    model.CategoryId,
                    restaurantId);

            if (category == null)
            {
                return (
                    false,
                    "Selected category is invalid."
                );
            }


            // =============================================
            // UPDATE BASIC DETAILS
            // =============================================

            menuItem.CategoryId = model.CategoryId;

            menuItem.Name = model.Name.Trim();

            menuItem.Description =
                string.IsNullOrWhiteSpace(
                    model.Description)
                ? null
                : model.Description.Trim();

            menuItem.Price = model.Price;

            menuItem.IsAvailable = model.IsAvailable;

            menuItem.IsActive = model.IsActive;


            // =============================================
            // UPDATE IMAGE
            // =============================================

            if (model.Image != null &&
                model.Image.Length > 0)
            {
                var imageResult =
                    await SaveImageAsync(model.Image);

                if (!imageResult.Success)
                {
                    return (
                        false,
                        imageResult.Message
                    );
                }


                // Delete old image
                DeleteImage(menuItem.ImagePath);


                // Save new image path
                menuItem.ImagePath =
                    imageResult.ImagePath;
            }


            // =============================================
            // SAVE DATABASE
            // =============================================

            await _menuItemRepository.UpdateAsync(menuItem);

            await _menuItemRepository.SaveChangesAsync();


            return (
                true,
                "Menu item updated successfully."
            );
        }


        // =====================================================
        // DELETE MENU ITEM
        // =====================================================

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


            // Soft Delete
            menuItem.IsActive = false;

            await _menuItemRepository.UpdateAsync(menuItem);

            await _menuItemRepository.SaveChangesAsync();

            return true;
        }


        // =====================================================
        // SAVE IMAGE HELPER
        // =====================================================

        private async Task<
            (bool Success, string Message, string? ImagePath)>
            SaveImageAsync(IFormFile image)
        {
            // Allowed extensions
            var allowedExtensions =
                new[]
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
                return (
                    false,
                    "Only JPG, JPEG, PNG and WEBP images are allowed.",
                    null
                );
            }


            // Maximum 5 MB
            if (image.Length > 5 * 1024 * 1024)
            {
                return (
                    false,
                    "Image size cannot be greater than 5 MB.",
                    null
                );
            }


            // Folder:
            // wwwroot/images/menuitems

            var uploadsFolder =
                Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "menuitems");


            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(
                    uploadsFolder);
            }


            // Unique filename
            var fileName =
                $"{Guid.NewGuid()}{extension}";


            var filePath =
                Path.Combine(
                    uploadsFolder,
                    fileName);


            // Save file
            using (var stream =
                   new FileStream(
                       filePath,
                       FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }


            // Database path
            var imagePath =
                $"/images/menuitems/{fileName}";


            return (
                true,
                "Image uploaded successfully.",
                imagePath
            );
        }


        // =====================================================
        // DELETE OLD IMAGE HELPER
        // =====================================================

        private void DeleteImage(
            string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }


            try
            {
                var relativePath =
                    imagePath.TrimStart('/');


                var filePath =
                    Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        relativePath);


                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Do not stop database update
                // if old image deletion fails
            }


        }


        // =====================================================
        // ACTIVATE MENU ITEM
        // =====================================================

        public async Task<bool> ActivateMenuItemAsync(
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

            menuItem.IsActive = true;

            await _menuItemRepository.UpdateAsync(menuItem);

            await _menuItemRepository.SaveChangesAsync();

            return true;
        }
        // =====================================================
        // UPDATE PRICE ONLY
        // =====================================================

        public async Task<bool> UpdatePriceAsync(
            int restaurantId,
            int menuItemId,
            decimal price)
        {
            var menuItem =
                await _menuItemRepository.GetByIdAsync(
                    menuItemId,
                    restaurantId);

            if (menuItem == null)
            {
                return false;
            }

            menuItem.Price = price;

            await _menuItemRepository.UpdateAsync(menuItem);

            await _menuItemRepository.SaveChangesAsync();

            return true;
        }
    }
}