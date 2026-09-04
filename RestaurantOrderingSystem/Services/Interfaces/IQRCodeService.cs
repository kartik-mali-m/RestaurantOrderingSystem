namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface IQRCodeService
    {
        Task<string> GenerateRestaurantQRCodeAsync(
            int restaurantId,
            string baseUrl);
    }
}