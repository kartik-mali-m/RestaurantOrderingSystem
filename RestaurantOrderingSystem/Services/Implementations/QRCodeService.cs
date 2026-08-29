using RestaurantOrderingSystem.Helpers;
using RestaurantOrderingSystem.Services.Interfaces;

namespace RestaurantOrderingSystem.Services.Implementations
{
    public class QRCodeService : IQRCodeService
    {
        private readonly IWebHostEnvironment _environment;

        public QRCodeService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> GenerateRestaurantQRCodeAsync(
            int restaurantId,
            string baseUrl)
        {
            // Create public menu URL
            string menuUrl =
                $"{baseUrl}/Menu/Restaurant/{restaurantId}";

            // Generate QR Code Image
            byte[] qrCodeBytes =
                QRCodeHelper.GenerateQRCode(menuUrl);

            // Create QR Code folder
            string qrFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "qrcodes");

            if (!Directory.Exists(qrFolder))
            {
                Directory.CreateDirectory(qrFolder);
            }

            // Generate unique file name
            string fileName =
                $"restaurant-{restaurantId}-{Guid.NewGuid()}.png";

            string filePath =
                Path.Combine(qrFolder, fileName);

            // Save QR Code Image
            await File.WriteAllBytesAsync(
                filePath,
                qrCodeBytes);

            // Return relative path
            return $"/uploads/qrcodes/{fileName}";
        }
    }
}