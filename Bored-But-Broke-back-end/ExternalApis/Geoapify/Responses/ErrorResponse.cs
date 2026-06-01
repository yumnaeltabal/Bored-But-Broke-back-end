using System.Text.Json.Serialization;

namespace Bored_But_Broke_back_end.ExternalApis.Geoapify.Responses
{
    public class ErrorResponse
    {
        [JsonPropertyName("statusCode")]
        public int? StatusCode { get; set; }

        [JsonPropertyName("error")]
        public string? ErrorTitle { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string? ErrorMessage { get; set; } = string.Empty;
    }
}