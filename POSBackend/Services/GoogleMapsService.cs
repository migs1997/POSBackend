using Newtonsoft.Json.Linq;

namespace POSBackend.Services
{
    public class GoogleMapsService
    {
        private readonly HttpClient _client = new();
        private readonly string _apiKey;

        public GoogleMapsService(IConfiguration config)
        {
            _apiKey = config["GoogleMaps:ApiKey"]
                ?? throw new Exception("Google Maps API key missing.");
        }

        public async Task<double> GetDistanceAsync(double originLat, double originLng, double destLat, double destLng)
        {
            string url =
                $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={originLat},{originLng}&destinations={destLat},{destLng}&key={_apiKey}&units=metric";

            var response = await _client.GetStringAsync(url);
            var json = JObject.Parse(response);

            double distanceMeters =
                json["rows"]?[0]?["elements"]?[0]?["distance"]?["value"]?.Value<double>()
                ?? throw new Exception("Distance not found");

            return distanceMeters / 1000; // km
        }
    }
}
