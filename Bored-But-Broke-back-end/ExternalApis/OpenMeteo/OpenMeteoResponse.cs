namespace Bored_But_Broke_back_end.ExternalApis.OpenMeteo
{
    public class OpenMeteoResponse
    {
        public HourlyData hourly { get; set; } = default!;
    }

    public class HourlyData
    {
        public string[] time { get; set; } = [];
        public int[] weather_code { get; set; } = [];
    }
}
