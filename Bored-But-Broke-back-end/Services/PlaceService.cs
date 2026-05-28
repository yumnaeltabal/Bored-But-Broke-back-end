using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.Models;
using Microsoft.Extensions.Primitives;

namespace Bored_But_Broke_back_end.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IYelpClient _yelpClient;
        public PlaceService(IYelpClient yelpClient) 
        { 
            _yelpClient = yelpClient;
        }
        public async Task<List<Place>> GetPlacesAsync(CancellationToken token)
        {
            var query = new Dictionary<string, StringValues>();

            query.TryAdd("location", "London");
            query.TryAdd("limit", "50");

            var response = await _yelpClient.GetPlacesAsync(query, token);

            return response.ToPlaces();
        }
    }
}
