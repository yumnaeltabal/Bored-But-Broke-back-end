using System.Text.Json.Serialization;

namespace Bored_But_Broke_back_end.ExternalApis.Geoapify.Responses
{
    public class GeocodeResponse
    {
        [JsonPropertyName("results")]
        public List<GeocodeResult>? Results { get; set; } = [];
    }

    public class GeocodeResult
    {
        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; } = string.Empty;

        [JsonPropertyName("lat")]
        public double? Latitude { get; set; }

        [JsonPropertyName("lon")]
        public double? Longitude { get; set; }
    }
}