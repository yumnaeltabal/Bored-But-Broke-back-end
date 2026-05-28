namespace Bored_But_Broke_back_end.Models
{
    public class GeoResponse
    {
        public GeoResult[] results { get; set; }
    }
    public class GeoResult
    {
        public string name { get; set; } = "";
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string country { get; set; } = "";
    }
}
