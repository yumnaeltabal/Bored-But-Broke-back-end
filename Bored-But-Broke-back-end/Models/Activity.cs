namespace Bored_But_Broke_back_end.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string MainCategory { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public Price Price { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Hours Hours { get; set; } = new Hours();
        public double Rating { get; set; }
        public string LocationUrl { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
