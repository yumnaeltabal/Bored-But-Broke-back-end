namespace Bored_But_Broke_back_end.Models
{
    public class WeatherHourly
    {
        public HourlyData hourly { get; set; } = default!;
    }

    public class HourlyData 
    {
        public string[] time { get; set; } = [];
        public double[] temperature_2m { get; set; } = [];
        public double[] precipotation { get; set; } = [];
        public double[] wind_gusts_10m { get; set; } = [];
    }
}
