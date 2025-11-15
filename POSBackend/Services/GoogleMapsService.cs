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
            string url = "https://api.openrouteservice.org/v2/directions/driving-car";

            var body = new
            {
                coordinates = new[]
                {
                    new[] { originLng, originLat },
                    new[] { destLng, destLat }
                }
            };

            string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

            Console.WriteLine("=== ORS Distance Request ===");
            Console.WriteLine("URL: " + url);
            Console.WriteLine("Body: " + jsonBody);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", _apiKey);
            request.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            string raw = await response.Content.ReadAsStringAsync();

            Console.WriteLine("ORS Raw Response: " + raw);

            if (!response.IsSuccessStatusCode)
                throw new Exception("ORS error: " + raw);

            var json = JObject.Parse(raw);

            double distanceMeters =
                json["routes"]?[0]?["summary"]?["distance"]?.Value<double>()
                ?? throw new Exception("Distance not found in ORS response.");

            Console.WriteLine("Distance (meters): " + distanceMeters);

            return distanceMeters / 1000; // convert to KM
        }
    }
}
