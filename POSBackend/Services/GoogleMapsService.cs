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
            var location = json["results"]?[0]?["geometry"]?["location"];
            if (location == null) throw new Exception("Address not found.");
            double lat = (double)location["lat"];
            double lng = (double)location["lng"];
            return (lat, lng);
        }

        // Get driving distance in km using Distance Matrix API
        public async Task<double> GetDistanceAsync(string origin, string destination)
        {
            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={Uri.EscapeDataString(origin)}&destinations={Uri.EscapeDataString(destination)}&key={_apiKey}&units=metric";
            var response = await _client.GetStringAsync(url);
            var json = JObject.Parse(response);
            var distanceMeters = json["rows"]?[0]?["elements"]?[0]?["distance"]?["value"];
            if (distanceMeters == null) throw new Exception("Unable to calculate distance.");
            return ((double)distanceMeters) / 1000; // Convert to km
        }
    }
}
