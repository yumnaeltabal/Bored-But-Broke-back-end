namespace Bored_But_Broke_back_end.ExternalApis.WeatherApi
{
    public class OpenMetroResponse
    {
        public HourlyData hourly { get; set; } = default!;
    }

    public class HourlyData
    {
        public string[] time { get; set; } = [];
        public double[] temperature_2m { get; set; } = [];
        public double[] precipitation { get; set; } = [];
        public double[] wind_gusts_10m { get; set; } = [];
    }
}
