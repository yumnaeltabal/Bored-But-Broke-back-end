using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.ExternalApis.Yelp.Extensions;
using Bored_But_Broke_back_end.Models;
using Bored_But_Broke_back_end.Models.Queries;
using Microsoft.Extensions.Primitives;

namespace Bored_But_Broke_back_end.Services
{
    public interface IPlaceService
    {
        Task<List<Place>> GetPlacesAsync(GetPlacesQuery query, CancellationToken ct);
    }
    public class PlaceService : IPlaceService
    {
        private readonly IYelpClient _yelpClient;
        private readonly ILocationService _locationService;
        public PlaceService(IYelpClient yelpClient, ILocationService locationService) 
        { 
            _yelpClient = yelpClient;
            _locationService = locationService;
        }
        public async Task<List<Place>> GetPlacesAsync(GetPlacesQuery query, CancellationToken token)
        {
            // TODO: add categories in query

            var coordinates = await _locationService.GetCoordinatesFromAddressAsync(query.Location, token) 
                ?? throw new BadHttpRequestException("Please enter a valid location in the UK.");
            
            var queryParams = new Dictionary<string, StringValues>();
            queryParams.Add("latitude", coordinates.Latitude.ToString());
            queryParams.Add("longitude", coordinates.Longitude.ToString());
            //queryParams.Add("categories", query.Categories);
            queryParams.Add("term", "Activity");
            queryParams.Add("radius", query.Radius.ToString());
            queryParams.Add("price", String.Join(",", Enumerable.Range(1, (int)query.Budget)));
            queryParams.Add("limit", query.Limit.ToString());

            var response = await _yelpClient.BusinessesSearchAsync(queryParams, token);

            return response.ToPlaces();
        }
    }
}
