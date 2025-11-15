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

        public async Task<double> GetDistanceAsync(string origin, string destination)
        {
            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={Uri.EscapeDataString(origin)}&destinations={Uri.EscapeDataString(destination)}&key={_apiKey}&units=metric";

            var response = await _client.GetStringAsync(url);
            var json = JObject.Parse(response);

            var distanceToken = json["rows"]?.FirstOrDefault()?["elements"]?.FirstOrDefault()?["distance"]?["value"];
            if (distanceToken == null) throw new Exception("Unable to calculate distance");

            return distanceToken.Value<double>() / 1000; // meters → km
        }
    }
}
