using Microsoft.AspNetCore.Http;

namespace RestaurantOrderingSystem.Services.Interfaces
{
    public interface IFileService
    {
        Task<string?> UploadRestaurantLogoAsync(
            IFormFile file,
            int restaurantId);

        Task DeleteFileAsync(string? filePath);
    }
}