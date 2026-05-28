namespace Bored_But_Broke_back_end.Services
{
    public interface IWeatherService
    {
        Task<object> GetWeatherAndForwardAsync(WeatherRequest request);
    }
}
