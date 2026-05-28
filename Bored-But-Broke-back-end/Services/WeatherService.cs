using Bored_But_Broke_back_end.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Bored_But_Broke_back_end.Services
{
    public class WeatherService : IWeatherService
    {
       private readonly HttpClient _httpClient;
        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherResult> GetWeatherAndForwardAsync(WeatherRequest request)
        {
            Console.WriteLine("WEATHER SERVICE BEGINNING");

            Console.WriteLine("Frontend request received:");
            Console.WriteLine($"city: {request.City}");
            Console.WriteLine($"date: {request.Date}");
            Console.WriteLine($"time: {request.Time}");

            var dateTime = DateTime.Parse($"{request.Date} {request.Time}");
            Console.WriteLine($"parsed DateTime: {dateTime}");

            var coordConverter = $"https://geocoding-api.open-meteo.com/v1/search?name={request.City}&count=1";
            Console.WriteLine("Calling Geocoding API...");
            Console.WriteLine($"geocoding URL: {coordConverter}");

            var coord = await _httpClient.GetFromJsonAsync<GeoResponse>(coordConverter);

            if (coord?.results == null || coord.results.Length == 0)
            {
                Console.WriteLine("ERROR: City not found");
                throw new Exception("City not found");
            }

            double lat = coord!.results[0].latitude;
            double lon = coord.results[0].longitude;

            var weatherUrl = $"https://api.open-meteo.com/v1/forecast" +
                             $"?latitude={lat}" +
                             $"&longitude={lon}" +
                             $"&hourly=temperature_2m" +
                             $"&start_date={dateTime:yyyy-MM-dd}" +
                             $"&end_date={dateTime:yyyy-MM-dd}";

            Console.WriteLine("Calling Weather API...");
            Console.WriteLine($"Weather URL: {weatherUrl}");

            var weather =
            await _httpClient.GetFromJsonAsync<WeatherHourly>(weatherUrl);
            if (weather == null)
            {
                Console.WriteLine("weather API failed");
                throw new Exception("weather API failed");
            }
        }
    }
}
