namespace Bored_But_Broke_back_end.Models.Responses
{
    public class PlaceResponse
    {
        public string PlaceId { get; set; } = string.Empty;
        public string PlaceName { get; set; } = string.Empty;
        public Location Location { get; set; } = new Location();
        public List<Category> Categories { get; set; } = new List<Category>();
        public Price? Price { get; set; }
        public Coordinates Coordinates { get; set; } = new Coordinates();
        public List<OpeningHours> OpeningHours { get; set; } = new List<OpeningHours>();
        public double? Rating { get; set; }
        public string PlaceUrl { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsFavourited { get; set; }
    }
}
