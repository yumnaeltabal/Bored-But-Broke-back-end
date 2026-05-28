using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bored_But_Broke_back_end.ExternalApis.Yelp
{
    public interface IYelpClient
    {
        public Task<SearchResponse> GetPlacesAsync(Dictionary<string, StringValues> query, CancellationToken token);
    }

    // TODO: rewrite console log to logger
    public class YelpClient : IYelpClient
    {
        private readonly HttpClient _httpClient;
        private static readonly string _searchEndPoint = "businesses/search";

        public YelpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SearchResponse> GetPlacesAsync(Dictionary<string, StringValues> query, CancellationToken token)
        {
            HttpResponseMessage response;

            var url = QueryHelpers.AddQueryString(_searchEndPoint, query);

            try
            {
                response = await _httpClient.GetAsync(url, token);
                Console.WriteLine(response);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Couldn't connect to Yelp API.", ex);
            }
            catch (TaskCanceledException)
            {
                throw new ExternalApiException(
                    statusCode: HttpStatusCode.GatewayTimeout,
                    errorTitle: "Gateway Timeout",
                    errorDetail: "Yelp API failed to respond in time."
                );
            }

            string responseBody = await response.Content.ReadAsStringAsync(token);
            Console.WriteLine(responseBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(responseBody);
                throw new ExternalApiException(
                    statusCode: response.StatusCode,
                    errorTitle: $"Yelp API error: {response.ReasonPhrase}",
                    errorDetail: error?.Error.Description
                );
            }

            return JsonSerializer.Deserialize<SearchResponse>(responseBody)
                ?? throw new ExternalApiException(
                    statusCode: HttpStatusCode.BadGateway,
                    errorTitle: "Bad Gateway",
                    errorDetail: "Failed to deserialize response from Yelp API."
                );
        }
    }
}
