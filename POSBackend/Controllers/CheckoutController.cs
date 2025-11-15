using Microsoft.AspNetCore.Mvc;
using POSBackend.Models;
using POSBackend.Services;

namespace POSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly GoogleMapsService _mapsService;

        // Your actual store coordinates
        private const double StoreLat = 14.728089;
        private const double StoreLng = 121.142296;

        public CheckoutController(GoogleMapsService mapsService)
        {
            _mapsService = mapsService;
        }

        [HttpPost("calculate-shipping")]
        public async Task<IActionResult> CalculateShipping([FromBody] LocationRequest request)
        {
            // Safe fallback for invalid coordinates
            if (request.Lat == 0 || request.Lng == 0)
            {
                return Ok(new { distanceKm = 0, shippingFee = 60 });
            }

            double distanceKm = await _mapsService.GetDistanceAsync(
                StoreLat, StoreLng,
                request.Lat, request.Lng
            );

            double shippingFee = distanceKm <= 3 ? 60 : 60 + Math.Ceiling(distanceKm - 3) * 10;

            return Ok(new { distanceKm, shippingFee });
        }

    }
}
