using Newtonsoft.Json.Linq;

namespace POSBackend.Services
{
    public class GoogleMapsService
    {
        private readonly HttpClient _client = new();
        private readonly string _apiKey;

        public GoogleMapsService(IConfiguration config)
        {
            _apiKey = config["OpenRouteService:ApiKey"]
                ?? throw new Exception("ORS API key missing.");
        }

        public async Task<double> GetDistanceAsync(double originLat, double originLng, double destLat, double destLng)
        {
            string url =
                $"https://api.openrouteservice.org/v2/directions/driving-car?" +
                $"api_key={_apiKey}&start={originLng},{originLat}&end={destLng},{destLat}";

            Console.WriteLine("Request URL: " + url);

            try
            {
                var response = await _client.GetStringAsync(url);
                Console.WriteLine("Raw response: " + response);

                var json = JObject.Parse(response);

                double distanceMeters =
                    json["features"]?[0]?["properties"]?["summary"]?["distance"]?.Value<double>()
                    ?? throw new Exception("Distance not found in ORS response");

                return distanceMeters / 1000; // convert meters → km
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ORS distance: " + ex);
                throw;
            }
        }

    }
}
