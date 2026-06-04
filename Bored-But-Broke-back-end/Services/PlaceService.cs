using Bored_But_Broke_back_end.Exceptions;
using Bored_But_Broke_back_end.ExternalApis.OpenMeteo;
using Bored_But_Broke_back_end.ExternalApis.Yelp;
using Bored_But_Broke_back_end.ExternalApis.Yelp.Extensions;
using Bored_But_Broke_back_end.Models;
using Bored_But_Broke_back_end.Models.Queries;
using Bored_But_Broke_back_end.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Numerics;
using System.Text.Json;

namespace Bored_But_Broke_back_end.Services
{
    public interface IPlaceService
    {
        Task<PlacesResponse> GetPlacesAsync(GetPlacesQuery query, HttpContext context, CancellationToken ct);
        Task<PlaceResponse> GetPlaceByIdAsync(string placeId, HttpContext context, CancellationToken ct);
    }
    public class PlaceService : IPlaceService
    {
        private static readonly string _allActivitiesPath = Path.Combine(AppContext.BaseDirectory, @"Models\Activities\AllActivities.json");
        private static readonly string _categoryPath = Path.Combine(AppContext.BaseDirectory, @"Models\Activities\ActivitiesByCategory.json");
        private static readonly string _weatherPath = Path.Combine(AppContext.BaseDirectory, @"Models\Activities\ActivitiesByWeather.json");
        private static readonly string _agePath = Path.Combine(AppContext.BaseDirectory, @"Models\Activities\ActivitiesByAge.json");

        private static readonly HashSet<string> AllActivities
            = JsonSerializer.Deserialize<HashSet<string>>(File.ReadAllText(_allActivitiesPath))
            ?? throw new InvalidOperationException("JSON file 'AllActivities.json' is missing.");
        private static readonly Dictionary<string, HashSet<string>> ActivitiesByCategory
            = JsonSerializer.Deserialize<Dictionary<string, HashSet<string>>>(File.ReadAllText(_categoryPath))
            ?? throw new InvalidOperationException("JSON file 'ActivitiesByCategory.json' is missing.");
        private static readonly Dictionary<string, HashSet<string>> ActivitiesByWeather
            = JsonSerializer.Deserialize<Dictionary<string, HashSet<string>>>(File.ReadAllText(_weatherPath))
            ?? throw new InvalidOperationException("JSON file 'ActivitiesByWeather.json' is missing.");
        private static readonly Dictionary<string, HashSet<string>> ActivitiesByAge
            = JsonSerializer.Deserialize<Dictionary<string, HashSet<string>>>(File.ReadAllText(_agePath))
            ?? throw new InvalidOperationException("JSON file 'ActivitiesByAge.json' is missing.");

        private readonly IYelpClient _yelpClient;
        private readonly ILocationService _locationService;
        private readonly IWeatherService _weatherService;
        private readonly IFavouriteService _favouriteService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlaceService(IYelpClient yelpClient, ILocationService locationService, 
            IWeatherService weatherService, IFavouriteService favouriteService,
            UserManager<ApplicationUser> userManager)
        {
            _yelpClient = yelpClient;
            _locationService = locationService;
            _weatherService = weatherService;
            _favouriteService = favouriteService;
            _userManager = userManager;
        }
        public async Task<PlacesResponse> GetPlacesAsync(GetPlacesQuery query, HttpContext context, CancellationToken token)
        {

            var coordinates = await _locationService.GetCoordinatesFromAddressAsync(query.Location, token)
                ?? throw new BadHttpRequestException("Please enter a valid location in the UK.");

            int startHour = query.StartTime.Hour;
            int endHour = query.EndTime.Hour;
            int count = endHour - startHour + 1;

            int[] hours = Enumerable.Range(startHour, count).ToArray();

            var weatherRequest = new WeatherRequest
            {
                lat = coordinates.Latitude!.Value,
                lon = coordinates.Longitude!.Value,
                Date = query.Date.ToString("yyyy-MM-dd"),
                Hours = hours
            };

            var isIndoor = await _weatherService.GetWeatherAndForwardAsync(weatherRequest);

            var filteredActivities = new HashSet<string>();

            if (query.Categories is not null)
            {
                foreach (var category in query.Categories.Split(","))
                {
                    if (ActivitiesByCategory.TryGetValue(category, out var activities)) filteredActivities.UnionWith(activities);
                }

                if (filteredActivities.Count == 0) filteredActivities = AllActivities;
            }
            else
            {
                filteredActivities = AllActivities;
            }

            if (isIndoor) filteredActivities.IntersectWith(ActivitiesByWeather["indoor"]);
            filteredActivities.IntersectWith(ActivitiesByAge[query.AgeRange.ToString().ToLowerInvariant()]);

            var queryParams = new Dictionary<string, StringValues>();
            queryParams.Add("latitude", coordinates.Latitude.ToString());
            queryParams.Add("longitude", coordinates.Longitude.ToString());
            queryParams.Add("categories", String.Join(",", filteredActivities));
            queryParams.Add("term", "Activity");
            queryParams.Add("radius", query.Radius.ToString());
            queryParams.Add("price", String.Join(",", Enumerable.Range(1, (int)query.Budget)));
            queryParams.Add("limit", query.Limit.ToString());

            var response = await _yelpClient.BusinessesSearchAsync(queryParams, token);

            var places = response.ToPlaces();

            var isFavouritedList = new bool[places.Count];
            var userId = _userManager.GetUserId(context.User);

            if (userId is not null) 
            {
                isFavouritedList = (bool[]) await _favouriteService.GetFavouriteStatusAsync(userId, places.Select(p => p.PlaceId));
            }

            return new PlacesResponse
            {
                Places = places.Zip(isFavouritedList, (p, f) => new PlaceResponse 
                { 
                    PlaceId = p.PlaceId,
                    PlaceName = p.PlaceName,
                    Location = p.Location,
                    Categories = p.Categories,
                    Price = p.Price,
                    Coordinates = p.Coordinates,
                    OpeningHours = p.OpeningHours,
                    Rating = p.Rating,
                    PlaceUrl = p.PlaceUrl,
                    ImageUrl = p.ImageUrl,
                    IsFavourited = f 
                }).ToList(),
                IsIndoor = isIndoor
            };
        }
        public async Task<PlaceResponse> GetPlaceByIdAsync(string placeId, HttpContext context, CancellationToken token)
        {
            var business = await _yelpClient.BusinessesGetByIdAsync(placeId, token)
                ?? throw new PlaceNotFoundException(placeId);

            var place = business.ToPlace();

            bool isFavourited = false;
            var userId = _userManager.GetUserId(context.User);

            if (userId is not null)
            {
                var status = await _favouriteService.GetFavouriteStatusAsync(userId, [place.PlaceId]);
                isFavourited = status.First();
            }

            return new PlaceResponse
            {
                PlaceId = place.PlaceId,
                PlaceName = place.PlaceName,
                Location = place.Location,
                Categories = place.Categories,
                Price = place.Price,
                Coordinates = place.Coordinates,
                OpeningHours = place.OpeningHours,
                Rating = place.Rating,
                PlaceUrl = place.PlaceUrl,
                ImageUrl = place.ImageUrl,
                IsFavourited = isFavourited
            };
        }
    }

}
