namespace RestaurantOrderingSystem.ViewModels.Auth
{
    public class LoginResponseVM
    {
        public string Token { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public int? RestaurantId { get; set; }
    }
}