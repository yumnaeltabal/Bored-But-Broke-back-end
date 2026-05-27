using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Text.Json;

namespace Bored_But_Broke_back_end.ExternalApis.Yelp
{
    public interface IYelpClient
    {
        public Task<SearchResponse?> GetPlacesAsync(Dictionary<string, StringValues> query, CancellationToken token);
    }

    public class YelpClient : IYelpClient
    {
        private readonly HttpClient _httpClient;
        private static readonly string _searchEndPoint = "businesses/search";

        public YelpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SearchResponse?> GetPlacesAsync(Dictionary<string, StringValues> query, CancellationToken token)
        {
            var url = QueryHelpers.AddQueryString(_searchEndPoint, query);

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, token);

                Debug.WriteLine(response);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync(token);

                Debug.WriteLine(responseBody);

                if (String.IsNullOrEmpty(responseBody))
                {
                    Console.WriteLine("Yelp Search API returns null.");
                    return null;
                }
                return JsonSerializer.Deserialize<SearchResponse>(responseBody);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"{ex.Message}");
                return null;
            }
        }
    }
}
