namespace Bored_But_Broke_back_end.ExternalApis.WeatherApi
{
    public class WeatherHourlyResult
    {
        public string Time { get; set; } = "";
        public double Temperature { get; set; }
        public double Precipitation { get; set; }
        public double WindGust { get; set; }
    }
}
