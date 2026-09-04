using QRCoder;

namespace RestaurantOrderingSystem.Helpers
{
    public static class QRCodeHelper
    {
        public static byte[] GenerateQRCode(string qrCodeText)
        {
            using var qrGenerator = new QRCodeGenerator();

            using var qrCodeData = qrGenerator.CreateQrCode(
                qrCodeText,
                QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }
    }
}