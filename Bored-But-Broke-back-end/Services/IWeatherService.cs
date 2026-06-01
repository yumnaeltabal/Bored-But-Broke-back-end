using Bored_But_Broke_back_end.ExternalApis.OpenMeteo;

namespace Bored_But_Broke_back_end.Services
{
    public interface IWeatherService
    {
        Task<List<WeatherHourlyResult>> GetWeatherAndForwardAsync(WeatherRequest request);
    }
}
