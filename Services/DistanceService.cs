//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Ma3ak.Models;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json.Linq;

//namespace Ma3ak.Services
//{
//    public class DistanceService : IDistanceService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _apiKey;

//        public DistanceService(HttpClient httpClient, IConfiguration configuration)
//        {
//            _httpClient = httpClient;
//            _apiKey = configuration["GoogleMaps:ApiKey"];
//        }

//        public async Task<double> CalculateDistance(string origin, string destination)
//        {
//            var requestUri = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin}&destination={destination}&key={_apiKey}";

//            var response = await _httpClient.GetStringAsync(requestUri);
//            var json = JObject.Parse(response);

//            var distance = json["routes"][0]["legs"][0]["distance"]["value"].Value<double>(); // distance in meters

//            return distance / 1000; // convert to kilometers
//        }

//        public async Task<List<(MaintenanceCenter Center, double Distance)>> GetClosestCentersAsync(string userLocation, List<MaintenanceCenter> centers)
//        {
//            var distances = new List<(MaintenanceCenter Center, double Distance)>();

//            foreach (var center in centers)
//            {
//                var distance = await CalculateDistance(userLocation, center.CenterLocation);
//                distances.Add((center, distance));
//            }

//            return distances.OrderBy(d => d.Distance).Take(5).ToList();
//        }
//    }
//}
