namespace Bored_But_Broke_back_end.Models
{
    public class Place
    {
        public string PlaceId { get; set; } = string.Empty;
        public string PlaceName { get; set; } = string.Empty;
        public Location Location { get; set; } = new Location();
        public List<Category> Categories { get; set; } = new List<Category>();
        public Price Price { get; set; } = Price.Cheap;
        public Coordinates Coordinates { get; set; } = new Coordinates();
        public OpeningHours OpeningHours { get; set; } = new OpeningHours();
        public double Rating { get; set; } = 1;
        public string PlaceUrl { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
