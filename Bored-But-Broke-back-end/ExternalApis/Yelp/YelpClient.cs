using Bored_But_Broke_back_end.ExternalApis.Yelp.Responses;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Text.Json;

namespace Bored_But_Broke_back_end.ExternalApis.Yelp
{
    public interface IYelpClient
    {
        public Task<SearchResponse> BusinessesSearchAsync(Dictionary<string, StringValues> query, CancellationToken token);
        public Task<YelpBusiness> BusinessesGetByIdAsync(string placeId, CancellationToken token);
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

        // TODO: refactor query into different params
        public async Task<SearchResponse> BusinessesSearchAsync(Dictionary<string, StringValues> query, CancellationToken token)
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
            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException("Yelp API failed to respond in time.", ex);
            }

            string responseBody = await response.Content.ReadAsStringAsync(token);

            if (!response.IsSuccessStatusCode)
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(responseBody);
                throw new ExternalApiException(
                    statusCode: response.StatusCode,
                    errorTitle: $"Yelp API error: {response.ReasonPhrase}",
                    errorDetail: error?.Error?.Description
                );
            }

            return JsonSerializer.Deserialize<SearchResponse>(responseBody)
                ?? throw new ExternalApiException(
                    statusCode: HttpStatusCode.BadGateway,
                    errorTitle: "Bad Gateway",
                    errorDetail: "Failed to deserialize response from Yelp API."
                );
        }
        public async Task<YelpBusiness> GetBusinessByIdAsync(string placeId, CancellationToken token)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync($"businesses/{placeId}", token);
                Console.WriteLine(response);
            }

            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Couldn't connect to Yelp API.", ex);
            }

            catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException("Yelp API failed to respond in time.", ex);
            }

            string responseBody = await response.Content.ReadAsStringAsync(token);
            if (!response.IsSuccessStatusCode)
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(responseBody);
                throw new ExternalApiException(
                    statusCode: response.StatusCode,
                    errorTitle: $"Yelp API error: {response.ReasonPhrase}",
                    errorDetail: error?.Error?.Description
                );
            }
            return JsonSerializer.Deserialize<YelpBusiness>(responseBody)
                ?? throw new ExternalApiException(
                    statusCode: HttpStatusCode.BadGateway,
                    errorTitle: "Bad Gateway",
                    errorDetail: "Failed to deserialize response from Yelp API."
                );
        }
}
