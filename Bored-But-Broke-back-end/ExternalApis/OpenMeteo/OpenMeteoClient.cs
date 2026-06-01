namespace Bored_But_Broke_back_end.ExternalApis.OpenMeteo
{
    public interface IOpenMeteoClient
    {
        Task<OpenMeteoResponse?> GetWeatherAsync(
        double latitude,
        double longitude,
        string date);
    }
    public class OpenMeteoClient : IOpenMeteoClient
    {
        private readonly HttpClient _httpClient;

        public OpenMeteoClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<OpenMeteoResponse?> GetWeatherAsync(double latitude, double longitude, string date)
        {
            var url =
            $"https://api.open-meteo.com/v1/forecast" +
            $"?latitude={latitude}" +
            $"&longitude={longitude}" +
            $"&hourly=weather_code" +
            $"&start_date={date}" +
            $"&end_date={date}";

            return await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);
        }

    }
}
