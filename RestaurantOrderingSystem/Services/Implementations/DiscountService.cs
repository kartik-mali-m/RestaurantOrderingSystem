using RestaurantOrderingSystem.Models.Offer;
using RestaurantOrderingSystem.Repositories.Interfaces;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Offer;

namespace RestaurantOrderingSystem.Services.Implementations
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMenuItemRepository _menuItemRepository;

        public DiscountService(
            IDiscountRepository discountRepository,
            IMenuItemRepository menuItemRepository)
        {
            _discountRepository = discountRepository;
            _menuItemRepository = menuItemRepository;
        }


        // =============================================
        // GET ALL DISCOUNTS
        // =============================================

        public async Task<List<Discount>> GetDiscountsAsync(
            int restaurantId)
        {
            return await _discountRepository
                .GetByRestaurantIdAsync(restaurantId);
        }


        // =============================================
        // GET DISCOUNT BY ID
        // =============================================

        public async Task<Discount?> GetDiscountAsync(
            int restaurantId,
            int discountId)
        {
            return await _discountRepository
                .GetByIdAsync(
                    discountId,
                    restaurantId);
        }


        // =============================================
        // CREATE DISCOUNT
        // =============================================

        public async Task<(bool Success, string Message)>
            CreateDiscountAsync(
                int restaurantId,
                DiscountVM model)
        {
            // =============================================
            // VALIDATE MENU ITEM
            // =============================================

            var menuItem =
                await _menuItemRepository.GetByIdAsync(
                    model.MenuItemId,
                    restaurantId);

            if (menuItem == null)
            {
                return (
                    false,
                    "Invalid menu item selected."
                );
            }


            // =============================================
            // CHECK EXISTING ACTIVE DISCOUNT
            // =============================================

            var existingDiscount =
                await _discountRepository
                    .GetActiveByMenuItemIdAsync(
                        restaurantId,
                        model.MenuItemId);

            if (existingDiscount != null)
            {
                return (
                    false,
                    "An active discount already exists for this menu item."
                );
            }


            // =============================================
            // CREATE DISCOUNT
            // =============================================

            var discount = new Discount
            {
                RestaurantId = restaurantId,

                MenuItemId = model.MenuItemId,

                Name = model.Name,

                DiscountPercentage =
                    model.DiscountPercentage,

                IsActive = model.IsActive
            };


            await _discountRepository.AddAsync(discount);

            await _discountRepository.SaveChangesAsync();


            return (
                true,
                "Discount created successfully."
            );
        }


        // =============================================
        // UPDATE DISCOUNT
        // =============================================

        public async Task<(bool Success, string Message)>
            UpdateDiscountAsync(
                int restaurantId,
                DiscountVM model)
        {
            var discount =
                await _discountRepository.GetByIdAsync(
                    model.Id,
                    restaurantId);

            if (discount == null)
            {
                return (
                    false,
                    "Discount not found."
                );
            }


            // =============================================
            // VALIDATE MENU ITEM
            // =============================================

            var menuItem =
                await _menuItemRepository.GetByIdAsync(
                    model.MenuItemId,
                    restaurantId);

            if (menuItem == null)
            {
                return (
                    false,
                    "Invalid menu item selected."
                );
            }


            // =============================================
            // UPDATE DATA
            // =============================================

            discount.MenuItemId =
                model.MenuItemId;

            discount.Name =
                model.Name;

            discount.DiscountPercentage =
                model.DiscountPercentage;

            discount.IsActive =
                model.IsActive;


            await _discountRepository.UpdateAsync(discount);

            await _discountRepository.SaveChangesAsync();


            return (
                true,
                "Discount updated successfully."
            );
        }


        // =============================================
        // DELETE / DEACTIVATE DISCOUNT
        // =============================================

        public async Task<bool> DeleteDiscountAsync(
            int restaurantId,
            int discountId)
        {
            var discount =
                await _discountRepository.GetByIdAsync(
                    discountId,
                    restaurantId);

            if (discount == null)
            {
                return false;
            }

            discount.IsActive = false;

            await _discountRepository.UpdateAsync(discount);

            await _discountRepository.SaveChangesAsync();

            return true;
        }
    }
}