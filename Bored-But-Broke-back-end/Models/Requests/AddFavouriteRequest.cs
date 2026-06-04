using System.ComponentModel.DataAnnotations;

namespace Bored_But_Broke_back_end.Models.Requests
{
    public class AddFavouriteRequest
    {
        [Required]
        public string PlaceId { get; set; } = string.Empty;
        [Required]
        public string PlaceName { get; set; } = string.Empty;
        [Required]
        public Location Location { get; set; } = new Location();
        [Required]
        public List<Category> Categories { get; set; } = new List<Category>();
        [Required]
        public Price? Price { get; set; }
        [Required]
        public Coordinates Coordinates { get; set; } = new Coordinates();
        [Required]
        public List<OpeningHours> OpeningHours { get; set; } = new List<OpeningHours>();
        [Required]
        public double? Rating { get; set; }
        [Required]
        public string PlaceUrl { get; set; } = string.Empty;
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
