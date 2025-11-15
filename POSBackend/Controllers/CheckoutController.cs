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

        // Updated store coordinates
        private const double StoreLat = 14.728808277383203;
        private const double StoreLng = 121.14048267183851;

        public CheckoutController(GoogleMapsService mapsService)
        {
            _mapsService = mapsService;
        }

        [HttpPost("calculate-shipping")]
        public async Task<IActionResult> CalculateShipping([FromBody] LocationRequest request)
        {
            Console.WriteLine("=== Shipping Calculation Triggered ===");
            Console.WriteLine("Request payload: " + System.Text.Json.JsonSerializer.Serialize(request));

            // Fallback for invalid coordinates
            if (request.Lat == 0 || request.Lng == 0)
            {
                Console.WriteLine("Invalid coordinates received, returning default fee");
                return Ok(new { distanceKm = 0, shippingFee = 60 });
            }

            try
            {
                double distanceKm = await _mapsService.GetDistanceAsync(
                    StoreLat, StoreLng,
                    request.Lat, request.Lng
                );
                Console.WriteLine("Distance calculated (km): " + distanceKm);

                double shippingFee = distanceKm <= 3 ? 60 : 60 + Math.Ceiling(distanceKm - 3) * 10;
                Console.WriteLine("Calculated shipping fee: " + shippingFee);

                return Ok(new { distanceKm, shippingFee });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Shipping calculation error: " + ex);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }



    }
}
