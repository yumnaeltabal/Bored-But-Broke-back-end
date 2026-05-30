namespace Bored_But_Broke_back_end.Models
{
    public class WeatherHourlyResult
    {
        public string Time { get; set; } = "";
        public double Temperature { get; set; }
        public double Precipitation { get; set; }
        public double WindGust { get; set; }
    }
}
