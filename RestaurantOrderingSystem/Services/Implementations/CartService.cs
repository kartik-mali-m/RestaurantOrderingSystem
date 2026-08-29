using Microsoft.AspNetCore.Http;
using RestaurantOrderingSystem.Services.Interfaces;
using RestaurantOrderingSystem.ViewModels.Customer;
using System.Text.Json;

namespace RestaurantOrderingSystem.Services.Implementations
{
    public class CartService : ICartService
    {
        private const string CartSessionKey = "RestaurantCart";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session =>
            _httpContextAccessor.HttpContext!.Session;

        public CartVM GetCart()
        {
            var cartJson = Session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(cartJson))
            {
                return new CartVM();
            }

            return JsonSerializer.Deserialize<CartVM>(cartJson)
                   ?? new CartVM();
        }

        private void SaveCart(CartVM cart)
        {
            string cartJson = JsonSerializer.Serialize(cart);

            Session.SetString(CartSessionKey, cartJson);
        }

        public void AddToCart(
            int restaurantId,
            string restaurantName,
            CartItemVM item)
        {
            var cart = GetCart();

            // If cart is empty, set restaurant information
            if (cart.RestaurantId == 0)
            {
                cart.RestaurantId = restaurantId;
                cart.RestaurantName = restaurantName;
            }

            // Prevent items from different restaurants
            if (cart.RestaurantId != restaurantId)
            {
                throw new InvalidOperationException(
                    "You cannot add items from different restaurants to the same cart.");
            }

            var existingItem = cart.Items
                .FirstOrDefault(x => x.MenuItemId == item.MenuItemId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Items.Add(item);
            }

            SaveCart(cart);
        }

        public void UpdateQuantity(
            int menuItemId,
            int quantity)
        {
            var cart = GetCart();

            var item = cart.Items
                .FirstOrDefault(x => x.MenuItemId == menuItemId);

            if (item == null)
            {
                return;
            }

            if (quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            SaveCart(cart);
        }

        public void RemoveFromCart(int menuItemId)
        {
            var cart = GetCart();

            var item = cart.Items
                .FirstOrDefault(x => x.MenuItemId == menuItemId);

            if (item != null)
            {
                cart.Items.Remove(item);
            }

            SaveCart(cart);
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }
    }
}