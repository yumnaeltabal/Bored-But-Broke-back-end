namespace Bored_But_Broke_back_end.Models
{
    public class GeoResponse
    {
        public GeoResult[] result { get; set; }
    }
    public class GeoResult
    {
        public string name { get; set; } = "";
        public string latitude { get; set; } = "";
        public string longitude { get; set; } = "";
        public string country { get; set; } = "";
    }
}
