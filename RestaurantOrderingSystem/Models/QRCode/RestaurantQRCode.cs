using RestaurantOrderingSystem.Models.Base;
using RestaurantEntity = RestaurantOrderingSystem.Models.Restaurant.Restaurant;

namespace RestaurantOrderingSystem.Models.QRCode
{
    public class RestaurantQRCode : BaseEntity
    {
        public int RestaurantId { get; set; }

        public string QRCodeValue { get; set; } = string.Empty;

        public string? QRCodeImagePath { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Property
        public RestaurantEntity Restaurant { get; set; } = null!;
    }
}