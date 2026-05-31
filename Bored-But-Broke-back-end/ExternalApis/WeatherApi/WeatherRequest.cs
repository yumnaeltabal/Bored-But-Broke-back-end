namespace Bored_But_Broke_back_end.ExternalApis.WeatherApi
{
    public class WeatherRequest
    {
        public double lon { get; set; }
        public double lat { get; set; }
        public string Date { get; set; }
        public int[] Hours { get; set; }
    }
}
