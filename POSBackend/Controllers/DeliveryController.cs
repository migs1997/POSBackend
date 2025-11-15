using DeliveryFeeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DeliveryFeeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {
        private readonly GoogleMapsService _mapsService;
        private readonly string _storeAddress;

        public DeliveryController(IConfiguration configuration)
        {
            _storeAddress = configuration["GoogleMaps:StoreAddress"]
                ?? throw new Exception("StoreAddress is missing in configuration.");

            string apiKey = configuration["GoogleMaps:ApiKey"]
                ?? throw new Exception("Google Maps API key is missing.");

            _mapsService = new GoogleMapsService(apiKey);
        }


        [HttpGet("fee")]
        public async Task<IActionResult> GetDeliveryFee([FromQuery] string customerAddress)
        {
            try
            {
                // Calculate distance
                double distance = await _mapsService.GetDistanceAsync(_storeAddress, customerAddress);

                // Calculate fee
                double fee = CalculateDeliveryFee(distance);

                return Ok(new
                {
                    distance,
                    fee
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private double CalculateDeliveryFee(double distance)
        {
            double baseFee = 50; // Base fee in your currency
            double perKm = 10;   // Fee per km
            return baseFee + (perKm * distance);
        }
    }
}
