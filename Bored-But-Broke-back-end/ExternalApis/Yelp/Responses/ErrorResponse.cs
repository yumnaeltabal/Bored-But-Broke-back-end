using System.Text.Json.Serialization;

namespace Bored_But_Broke_back_end.ExternalApis.Yelp.Responses
{
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public Error? Error { get; set; } = new();
    }

    public class Error
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; } = string.Empty;
    }
}
