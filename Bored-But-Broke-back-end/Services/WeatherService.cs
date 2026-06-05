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

        public async Task<bool> GetWeatherAndForwardAsync(WeatherRequest request)
        {

            var weather = await _client.GetWeatherAsync(request.lat,
                                                        request.lon,
                                                        request.Date);

            if (weather == null || weather.hourly == null)
                throw new Exception("Weather API failed");

            int badHours = 0;
            int totalHours = 0;

            foreach (var hour in request.Hours)
            {
                var targetTime = $"{request.Date}T{hour:D2}:00";
                var index = Array.IndexOf(weather.hourly.time, targetTime);

                if (index == -1)
                {
                    continue;
                }
                totalHours++;

               if (weather.hourly.weather_code[index] >= 51)
                {
                    badHours++;
                }

               
            }
            if (totalHours == 0)
                {
                    throw new Exception("You didnt give a right time");
                }

                var badWeatherPercentage = (double)badHours / (double)totalHours * 100;

                bool isIndoor = badWeatherPercentage >= 40;
                
                return isIndoor;
        }
    }
}
