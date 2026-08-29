using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingSystem.Services.Interfaces;
using System.Security.Claims;

namespace RestaurantOrderingSystem.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [Authorize]
    public class QRCodeController : Controller
    {
        private readonly IQRCodeService _qrCodeService;

        public QRCodeController(IQRCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate()
        {
            string? restaurantIdValue =
                User.FindFirst("RestaurantId")?.Value;

            if (string.IsNullOrEmpty(restaurantIdValue))
            {
                TempData["Error"] = "RestaurantId claim was not found.";
                return RedirectToAction(nameof(Index));
            }

            int restaurantId = int.Parse(restaurantIdValue);

            string baseUrl =
                $"{Request.Scheme}://{Request.Host}";

            string qrCodePath =
                await _qrCodeService.GenerateRestaurantQRCodeAsync(
                    restaurantId,
                    baseUrl);

            TempData["QRCodePath"] = qrCodePath;

            return RedirectToAction(nameof(Index));
        }
    }
}