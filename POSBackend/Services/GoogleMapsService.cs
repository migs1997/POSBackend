using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace DeliveryFeeAPI.Services
{
    public class GoogleMapsService
    {
        private readonly string _apiKey;
        private readonly HttpClient _client;

        public GoogleMapsService(string apiKey)
        {
            _apiKey = apiKey;
            _client = new HttpClient();
        }

        // Get coordinates for an address
        public async Task<(double lat, double lng)> GetCoordinatesAsync(string address)
        {
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";
            var response = await _client.GetStringAsync(url);
            var json = JObject.Parse(response);

            var location = json["results"]?.FirstOrDefault()?["geometry"]?["location"];
            if (location == null || location["lat"] == null || location["lng"] == null)
                throw new Exception("Address not found.");

            double lat = location["lat"]?.Value<double>() ?? throw new Exception("Latitude not found.");
            double lng = location["lng"]?.Value<double>() ?? throw new Exception("Longitude not found.");
            return (lat, lng);
        }

        // Get driving distance in km using Distance Matrix API
        public async Task<double> GetDistanceAsync(string origin, string destination)
        {
            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={Uri.EscapeDataString(origin)}&destinations={Uri.EscapeDataString(destination)}&key={_apiKey}&units=metric";
            var response = await _client.GetStringAsync(url);
            var json = JObject.Parse(response);

            var distanceToken = json["rows"]?.FirstOrDefault()?["elements"]?.FirstOrDefault()?["distance"]?["value"];
            if (distanceToken == null)
                throw new Exception("Unable to calculate distance.");

            double distanceMeters = distanceToken.Value<double>();
            return distanceMeters / 1000; // Convert to km
        }
    }
}
