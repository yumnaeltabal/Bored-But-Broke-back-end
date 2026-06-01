using Bored_But_Broke_back_end.ExternalApis.Geoapify.Responses;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Text.Json;

namespace Bored_But_Broke_back_end.ExternalApis.Geoapify
{
    public interface IGeoapifyClient
    {
        public Task<GeocodeResponse> ForwardGeocodingAsync(string address, CancellationToken token);
    }

    // TODO: rewrite console log to logger
    // TODO: Empty list for no results
    public class GeoapifyClient : IGeoapifyClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private static readonly string _geocodingEndPoint = "geocode/search";

        public GeoapifyClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GEOAPIFY_API_KEY"]
                    ?? throw new InvalidOperationException("Environment variable 'GEOAPIFY_API_KEY' is missing.");
        }
        public async Task<GeocodeResponse> ForwardGeocodingAsync(string address, CancellationToken token)
        {
            HttpResponseMessage response;

            var query = new Dictionary<string, StringValues>();
            query.Add("apiKey", _apiKey);
            query.Add("text", address);
            query.Add("lang", "en");
            query.Add("limit", "1");
            query.Add("format", "json");

            var url = QueryHelpers.AddQueryString(_geocodingEndPoint, query);

            try
            {
                response = await _httpClient.GetAsync(url, token);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Couldn't connect to Geoapify API.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException("Geoapify API failed to respond in time.", ex);
            }

            string responseBody = await response.Content.ReadAsStringAsync(token);

            if (!response.IsSuccessStatusCode)
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(responseBody);
                throw new ExternalApiException(
                    statusCode: response.StatusCode,
                    errorTitle: $"Geoapify API error: {response.ReasonPhrase}",
                    errorDetail: error?.ErrorMessage
                );
            }

            return JsonSerializer.Deserialize<GeocodeResponse>(responseBody)
                ?? throw new ExternalApiException(
                    statusCode: HttpStatusCode.BadGateway,
                    errorTitle: "Bad Gateway",
                    errorDetail: "Failed to deserialize response from Geoapify API."
                );
        }
    }
}
