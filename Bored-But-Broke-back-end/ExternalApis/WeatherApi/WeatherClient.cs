namespace Bored_But_Broke_back_end.ExternalApis.WeatherApi
{
    public interface IWeatherClient
    {
        Task<OpenMetroResponse?> GetWeatherAsync(
        double latitude,
        double longitude,
        string date);
    }
    public class WeatherClient : IWeatherClient
    {
        private readonly HttpClient _httpClient;

        public WeatherClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<OpenMetroResponse?> GetWeatherAsync(double latitude, double longitude, string date)
        {
            var url =
            $"https://api.open-meteo.com/v1/forecast" +
            $"?latitude={latitude}" +
            $"&longitude={longitude}" +
            $"&hourly=temperature_2m,precipitation,wind_gusts_10m" +
            $"&start_date={date}" +
            $"&end_date={date}";

            Console.WriteLine($"[OpenMeteoClient] {url}");

            return await _httpClient.GetFromJsonAsync<OpenMetroResponse>(url);
        }

    }
}
