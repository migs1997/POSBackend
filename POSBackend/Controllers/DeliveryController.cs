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
            if (string.IsNullOrWhiteSpace(customerAddress))
                return BadRequest(new { error = "Customer address is required." });

            try
            {
                // Calculate distance
                double distance = await _mapsService.GetDistanceAsync(_storeAddress, customerAddress);

                // Optional: round distance to 2 decimals
                distance = Math.Round(distance, 2);

                // Calculate fee
                double fee = CalculateDeliveryFee(distance);

                return Ok(new
                {
                    distance,
                    fee
                });
            }
            catch (HttpRequestException)
            {
                // Network issues or Google API down
                return StatusCode(503, new { error = "Unable to reach Google Maps API. Please try again later." });
            }
            catch (Exception ex)
            {
                // API returned invalid data or address not found
                return BadRequest(new { error = ex.Message });
            }
        }

        private double CalculateDeliveryFee(double distance)
        {
            double baseFee = 60; // Base fee in your currency
            double perKm = 10;   // Fee per km
            return baseFee + (perKm * distance);
        }
    }
}
