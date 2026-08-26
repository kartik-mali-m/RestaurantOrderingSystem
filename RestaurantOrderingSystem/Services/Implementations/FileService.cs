using Microsoft.AspNetCore.Http;
using RestaurantOrderingSystem.Services.Interfaces;

namespace RestaurantOrderingSystem.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> UploadRestaurantLogoAsync(
            IFormFile file,
            int restaurantId)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException(
                    "Only JPG, JPEG, PNG and WEBP images are allowed.");
            }

            if (file.Length > 2 * 1024 * 1024)
            {
                throw new InvalidOperationException(
                    "Logo size cannot exceed 2 MB.");
            }

            var folderPath = Path.Combine(
                _environment.WebRootPath,
                "images",
                "restaurants");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName =
                $"restaurant-{restaurantId}-{Guid.NewGuid():N}{extension}";

            var filePath =
                Path.Combine(folderPath, fileName);

            using var stream =
                new FileStream(
                    filePath,
                    FileMode.Create);

            await file.CopyToAsync(stream);

            return $"/images/restaurants/{fileName}";
        }

        public Task DeleteFileAsync(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Task.CompletedTask;
            }

            var physicalPath = Path.Combine(
                _environment.WebRootPath,
                filePath.TrimStart('/'));

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            return Task.CompletedTask;
        }
    }
}