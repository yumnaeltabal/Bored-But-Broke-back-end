using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.Models;
using Bored_But_Broke_back_end.Models.Queries;
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
        public async Task<List<Place>> GetPlacesAsync(GetPlacesQuery query, CancellationToken token)
        {
            // TODO: Convert location to coordinates
            // TODO: add categories in query

            var queryParams = new Dictionary<string, StringValues>();
            queryParams.Add("location", query.Location);
            queryParams.Add("radius", query.Radius.ToString());
            queryParams.Add("price", String.Join(",", query.Budget.Cast<int>()));
            queryParams.Add("limit", query.Limit.ToString());

            var response = await _yelpClient.GetPlacesAsync(queryParams, token);

            return response.ToPlaces();
        }
    }
}
