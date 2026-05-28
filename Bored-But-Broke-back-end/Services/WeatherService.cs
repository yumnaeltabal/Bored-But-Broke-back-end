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

        public async Task<object> GetWeatherAndForwardAsync(WeatherRequest request)
        {
            var dateTime = DateTime.Parse($"{request.Date} {request.Time}");



        }
    }
}
