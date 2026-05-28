using System.Globalization;

namespace Bored_But_Broke_back_end.Models
{
    public class WeatherRequest
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
