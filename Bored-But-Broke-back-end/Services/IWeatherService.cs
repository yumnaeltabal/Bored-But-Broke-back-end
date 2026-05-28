using Bored_But_Broke_back_end.Models;

namespace Bored_But_Broke_back_end.Services
{
    public interface IWeatherService
    {
        Task<WeatherResult> GetWeatherAndForwardAsync(WeatherRequest request);
    }
}
