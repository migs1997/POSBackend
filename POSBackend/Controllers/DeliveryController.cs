using POSBackend.Models;
using POSBackend.Services;

using Microsoft.AspNetCore.Mvc;

namespace DeliveryFeeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly GoogleMapsService _mapsService;
        private readonly IConfiguration _config;

        public CheckoutController(GoogleMapsService mapsService, IConfiguration config)
        {
            _mapsService = mapsService;
            _config = config;
        }

        [HttpPost("calculate-shipping")]
        public async Task<IActionResult> CalculateShipping([FromBody] LocationRequest request)
        {
            double storeLat = _config.GetValue<double>("GoogleMaps:StoreLat");
            double storeLng = _config.GetValue<double>("GoogleMaps:StoreLng");

            if (request.Lat == 0 || request.Lng == 0)
                return BadRequest(new { error = "Invalid customer location" });

            try
            {
                double distanceKm = await _mapsService.GetDistanceAsync(
                    storeLat, storeLng,
                    request.Lat, request.Lng
                );

                double shippingFee = distanceKm <= 3
                    ? 60
                    : 60 + Math.Ceiling(distanceKm - 3) * 10;

                return Ok(new { distanceKm, shippingFee });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
