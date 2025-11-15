using DeliveryFeeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DeliveryFeeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly GoogleMapsService _mapsService;
        private readonly string _storeAddress;

        public CheckoutController(IConfiguration configuration)
        {
            _storeAddress = configuration["GoogleMaps:StoreAddress"]
                ?? throw new Exception("StoreAddress is missing in configuration.");

            string apiKey = configuration["GoogleMaps:ApiKey"]
                ?? throw new Exception("Google Maps API key is missing.");

            _mapsService = new GoogleMapsService(apiKey);
        }

        [HttpPost("calculate-total")]
        public async Task<IActionResult> CalculateTotal([FromBody] CheckoutRequest request)
        {
            try
            {
                // Calculate distance and delivery fee
                double distance = await _mapsService.GetDistanceAsync(_storeAddress, request.CustomerAddress);
                double deliveryFee = CalculateDeliveryFee(distance);

                // Sum items in checkout
                double itemsTotal = request.Items.Sum(i => i.Price * i.Quantity);

                double total = itemsTotal + deliveryFee;

                return Ok(new
                {
                    itemsTotal,
                    deliveryFee,
                    total
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

    public class CheckoutRequest
    {
        public required string CustomerAddress { get; set; }
        public required List<CheckoutItem> Items { get; set; }
    }

    public class CheckoutItem
    {
        public required string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }

}
