using RestaurantOrderingSystem.Models.Menu;
using RestaurantOrderingSystem.ViewModels.Menu;
namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface IMenuService
    {
        // =====================================================
        // CATEGORY
        // =====================================================

        Task<List<Category>> GetCategoriesAsync(
            int restaurantId);

        Task<Category?> GetCategoryAsync(
            int restaurantId,
            int categoryId);

        Task<(bool Success, string Message)>
            CreateCategoryAsync(
                int restaurantId,
                CategoryVM model);

        Task<(bool Success, string Message)>
            UpdateCategoryAsync(
                int restaurantId,
                CategoryVM model);

        Task<bool> DeleteCategoryAsync(
            int restaurantId,
            int categoryId);


        // =====================================================
        // MENU ITEMS
        // =====================================================

        Task<List<MenuItem>> GetMenuItemsAsync(
            int restaurantId);

        Task<MenuItem?> GetMenuItemAsync(
            int restaurantId,
            int menuItemId);

        Task<(bool Success, string Message)>
            CreateMenuItemAsync(
                int restaurantId,
                MenuItemVM model);

        Task<(bool Success, string Message)>
            UpdateMenuItemAsync(
                int restaurantId,
                MenuItemVM model);

        Task<bool> DeleteMenuItemAsync(
            int restaurantId,
            int menuItemId);
    }
}