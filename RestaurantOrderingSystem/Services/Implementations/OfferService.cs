using RestaurantOrderingSystem.Models.Offer;
using RestaurantOrderingSystem.Repositories.Interfaces;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Offer;

namespace RestaurantOrderingSystem.Services.Implementations
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMenuItemRepository _menuItemRepository;

        public OfferService(
            IOfferRepository offerRepository,
            ICategoryRepository categoryRepository,
            IMenuItemRepository menuItemRepository)
        {
            _offerRepository = offerRepository;
            _categoryRepository = categoryRepository;
            _menuItemRepository = menuItemRepository;
        }


        // =============================================
        // GET ALL OFFERS
        // =============================================

        public async Task<List<Offer>> GetOffersAsync(
            int restaurantId)
        {
            return await _offerRepository
                .GetByRestaurantIdAsync(restaurantId);
        }


        // =============================================
        // GET OFFER BY ID
        // =============================================

        public async Task<Offer?> GetOfferAsync(
            int restaurantId,
            int offerId)
        {
            return await _offerRepository
                .GetByIdAsync(
                    offerId,
                    restaurantId);
        }


        // =============================================
        // CREATE OFFER
        // =============================================

        public async Task<(bool Success, string Message)>
            CreateOfferAsync(
                int restaurantId,
                OfferVM model)
        {
            // =============================================
            // DATE VALIDATION
            // =============================================

            if (model.StartDate >= model.EndDate)
            {
                return (
                    false,
                    "End date must be greater than start date."
                );
            }


            // =============================================
            // CATEGORY OFFER VALIDATION
            // =============================================

            if (model.TargetType == OfferTargetType.Category)
            {
                if (!model.CategoryId.HasValue)
                {
                    return (
                        false,
                        "Please select a category."
                    );
                }

                var category =
                    await _categoryRepository.GetByIdAsync(
                        model.CategoryId.Value,
                        restaurantId);

                if (category == null)
                {
                    return (
                        false,
                        "Invalid category selected."
                    );
                }
            }


            // =============================================
            // MENU ITEM OFFER VALIDATION
            // =============================================

            if (model.TargetType == OfferTargetType.MenuItem)
            {
                if (!model.MenuItemId.HasValue)
                {
                    return (
                        false,
                        "Please select a menu item."
                    );
                }

                var menuItem =
                    await _menuItemRepository.GetByIdAsync(
                        model.MenuItemId.Value,
                        restaurantId);

                if (menuItem == null)
                {
                    return (
                        false,
                        "Invalid menu item selected."
                    );
                }
            }


            // =============================================
            // CREATE OFFER
            // =============================================

            var offer = new Offer
            {
                RestaurantId = restaurantId,

                Name = model.Name,

                Description = model.Description,

                TargetType = model.TargetType,

                CategoryId =
                    model.TargetType == OfferTargetType.Category
                        ? model.CategoryId
                        : null,

                MenuItemId =
                    model.TargetType == OfferTargetType.MenuItem
                        ? model.MenuItemId
                        : null,

                DiscountPercentage =
                    model.DiscountPercentage,

                StartDate = model.StartDate,

                EndDate = model.EndDate,

                IsActive = model.IsActive
            };


            await _offerRepository.AddAsync(offer);

            await _offerRepository.SaveChangesAsync();


            return (
                true,
                "Offer created successfully."
            );
        }


        // =============================================
        // UPDATE OFFER
        // =============================================

        public async Task<(bool Success, string Message)>
            UpdateOfferAsync(
                int restaurantId,
                OfferVM model)
        {
            var offer =
                await _offerRepository.GetByIdAsync(
                    model.Id,
                    restaurantId);

            if (offer == null)
            {
                return (
                    false,
                    "Offer not found."
                );
            }


            // =============================================
            // DATE VALIDATION
            // =============================================

            if (model.StartDate >= model.EndDate)
            {
                return (
                    false,
                    "End date must be greater than start date."
                );
            }


            // =============================================
            // CATEGORY VALIDATION
            // =============================================

            if (model.TargetType == OfferTargetType.Category)
            {
                if (!model.CategoryId.HasValue)
                {
                    return (
                        false,
                        "Please select a category."
                    );
                }

                var category =
                    await _categoryRepository.GetByIdAsync(
                        model.CategoryId.Value,
                        restaurantId);

                if (category == null)
                {
                    return (
                        false,
                        "Invalid category selected."
                    );
                }
            }


            // =============================================
            // MENU ITEM VALIDATION
            // =============================================

            if (model.TargetType == OfferTargetType.MenuItem)
            {
                if (!model.MenuItemId.HasValue)
                {
                    return (
                        false,
                        "Please select a menu item."
                    );
                }

                var menuItem =
                    await _menuItemRepository.GetByIdAsync(
                        model.MenuItemId.Value,
                        restaurantId);

                if (menuItem == null)
                {
                    return (
                        false,
                        "Invalid menu item selected."
                    );
                }
            }


            // =============================================
            // UPDATE DATA
            // =============================================

            offer.Name = model.Name;

            offer.Description = model.Description;

            offer.TargetType = model.TargetType;

            offer.CategoryId =
                model.TargetType == OfferTargetType.Category
                    ? model.CategoryId
                    : null;

            offer.MenuItemId =
                model.TargetType == OfferTargetType.MenuItem
                    ? model.MenuItemId
                    : null;

            offer.DiscountPercentage =
                model.DiscountPercentage;

            offer.StartDate = model.StartDate;

            offer.EndDate = model.EndDate;

            offer.IsActive = model.IsActive;


            await _offerRepository.UpdateAsync(offer);

            await _offerRepository.SaveChangesAsync();


            return (
                true,
                "Offer updated successfully."
            );
        }


        // =============================================
        // DELETE / DEACTIVATE OFFER
        // =============================================

        public async Task<bool> DeleteOfferAsync(
            int restaurantId,
            int offerId)
        {
            var offer =
                await _offerRepository.GetByIdAsync(
                    offerId,
                    restaurantId);

            if (offer == null)
            {
                return false;
            }

            offer.IsActive = false;

            await _offerRepository.UpdateAsync(offer);

            await _offerRepository.SaveChangesAsync();

            return true;
        }
    }
}