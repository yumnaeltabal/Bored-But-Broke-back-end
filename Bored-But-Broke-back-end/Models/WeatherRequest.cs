using System.Globalization;

namespace Bored_But_Broke_back_end.Models
{
    public class WeatherRequest
    {
        public double lon { get; set; }
        public double lat { get; set; }
        public string Date { get; set; }
        public int[] Hours { get; set; }
    }
}
