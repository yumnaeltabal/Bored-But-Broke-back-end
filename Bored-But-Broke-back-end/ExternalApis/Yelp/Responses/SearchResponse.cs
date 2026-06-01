using System.Text.Json.Serialization;

namespace Bored_But_Broke_back_end.ExternalApis.Yelp.Responses
{
    public class SearchResponse
    {
        [JsonPropertyName("businesses")]
        public List<YelpBusiness>? Businesses { get; set; } = [];
    }
    public class YelpBusiness
    {
        [JsonPropertyName("id")]
        public string? PlaceId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? PlaceName { get; set; } = string.Empty;

        [JsonPropertyName("location")]
        public YelpLocation? Location { get; set; } = new();

        [JsonPropertyName("categories")]
        public List<YelpCategory>? Categories { get; set; } = [];

        [JsonPropertyName("price")]
        public string? Price { get; set; } = string.Empty;

        [JsonPropertyName("coordinates")]
        public YelpCoordinates? Coordinates { get; set; } = new();

        [JsonPropertyName("distance")]
        public double? Distance { get; set; }

        [JsonPropertyName("business_hours")]
        public List<YelpBusinessHours>? BusinessHours { get; set; } = new();

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("attributes")]
        public YelpAttributes? Attributes { get; set; } = new();

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; } = string.Empty;
    }

    public class YelpLocation
    {
        [JsonPropertyName("address1")]
        public string? Address1 { get; set; } = string.Empty;

        [JsonPropertyName("address2")]
        public string? Address2 { get; set; } = string.Empty;

        [JsonPropertyName("address3")]
        public string? Address3 { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string? City { get; set; } = string.Empty;

        [JsonPropertyName("zip_code")]
        public string? ZipCode { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string? Country { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string? State { get; set; } = string.Empty;

        [JsonPropertyName("display_address")]
        public List<string>? DisplayAddress { get; set; } = [];
    }

    public class YelpCategory
    {
        [JsonPropertyName("alias")]
        public string? Alias { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string? Title { get; set; } = string.Empty;
    }

    public class YelpCoordinates
    {
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }

    public class YelpBusinessHours
    {
        [JsonPropertyName("open")]
        public List<YelpHour>? Hours { get; set; } = [];

        [JsonPropertyName("hours_type")]
        public string? HoursType { get; set; } = string.Empty;

        [JsonPropertyName("is_open_now")]
        public bool? IsOpenNow { get; set; }
    }

    public class YelpHour
    {
        [JsonPropertyName("start")]
        public string? Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string? End { get; set; } = string.Empty;

        [JsonPropertyName("day")]
        public int? Day { get; set; }
    }

    public class YelpAttributes
    {
        [JsonPropertyName("business_url")]
        public string? PlaceUrl { get; set; } = string.Empty;
    }
}
