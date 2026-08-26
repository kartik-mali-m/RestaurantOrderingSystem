using RestaurantOrderingSystem.Models.Base;

namespace RestaurantOrderingSystem.Models.Identity
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public int? RestaurantId { get; set; }

        public Role Role { get; set; } = null!;

        public RestaurantOrderingSystem.Models.Restaurant.Restaurant? Restaurant { get; set; }
    }
}