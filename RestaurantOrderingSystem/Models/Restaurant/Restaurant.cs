using RestaurantOrderingSystem.Models.Base;
using RestaurantOrderingSystem.Models.Identity;

namespace RestaurantOrderingSystem.Models.Restaurant
{
    public class Restaurant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? GSTNumber { get; set; }

        public string Status { get; set; } = "Pending";

        // Restaurant logo file path
        // Example: /uploads/restaurants/abc123.png
        public string? LogoPath { get; set; }

        // Restaurant can have one or more users
        // Currently we will use this for RestaurantAdmin
        public ICollection<User> Users { get; set; }
            = new List<User>();
    }
}