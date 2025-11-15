using DeliveryFeeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryFeeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly GoogleMapsService _mapsService;
        private readonly string _storeAddress = "Bucad Residence 865 Libongco 3 St, Manggahan, Rodriguez, Rizal, Philippines";

        public CheckoutController(GoogleMapsService mapsService)
        {
            _mapsService = mapsService;
        }

        [HttpPost("calculate-shipping")]
        public async Task<IActionResult> CalculateShipping([FromBody] ShippingRequest request)
        {
            if (string.IsNullOrEmpty(request.CustomerAddress))
                return BadRequest(new { error = "Customer address is required" });

            try
            {
                double distanceKm = await _mapsService.GetDistanceAsync(_storeAddress, request.CustomerAddress);

                // Simple shipping fee: 60 for first 3km, +10 per extra km
                double shippingFee = distanceKm <= 3 ? 60 : 60 + Math.Ceiling(distanceKm - 3) * 10;

                return Ok(new { distanceKm, shippingFee });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class ShippingRequest
    {
        public required string CustomerAddress { get; set; }
    }
}
