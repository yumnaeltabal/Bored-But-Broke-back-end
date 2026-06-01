using Bored_But_Broke_back_end.ExternalApis.OpenMeteo;
using Bored_But_Broke_back_end.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Bored_But_Broke_back_end.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IOpenMeteoClient _client;
        public WeatherService(IOpenMeteoClient client)
        {
            _client = client;
        }

        public async Task<List<WeatherHourlyResult>> GetWeatherAndForwardAsync(WeatherRequest request)
        {

            var weather = await _client.GetWeatherAsync(request.lat,
                                                        request.lon,
                                                        request.Date);

            if (weather == null || weather.hourly == null)
                throw new Exception("Weather API failed");

            var results = new List<WeatherHourlyResult>();

            foreach (var hour in request.Hours)
            {
                var targetTime = $"{request.Date}T{hour:D2}:00";
                var index = Array.IndexOf(weather.hourly.time, targetTime);

                if (index == -1)
                {
                    continue;
                }
                results.Add(new WeatherHourlyResult
                {
                    Time = targetTime,
                });
            }
            return results;
        }
    }
}
