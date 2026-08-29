using RestaurantOrderingSystem.ViewModels.Customer;

namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface ICartService
    {
        CartVM GetCart();

        void AddToCart(
            int restaurantId,
            string restaurantName,
            CartItemVM item);

        void UpdateQuantity(
            int menuItemId,
            int quantity);

        void RemoveFromCart(int menuItemId);

        void ClearCart();
    }
}