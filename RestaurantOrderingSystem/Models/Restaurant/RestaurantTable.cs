using RestaurantOrderingSystem.Models.Base;

namespace RestaurantOrderingSystem.Models.Restaurant
{
    public class RestaurantTable : BaseEntity
    {
        public int RestaurantId { get; set; }

        public string TableNumber { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Navigation Property
        public Restaurant Restaurant { get; set; } = null!;
    }
}