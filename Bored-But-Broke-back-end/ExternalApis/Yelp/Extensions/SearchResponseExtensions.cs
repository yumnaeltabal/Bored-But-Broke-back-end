using Bored_But_Broke_back_end.ExternalApis.Yelp.Responses;
using Bored_But_Broke_back_end.Models;
using System.Net;

namespace Bored_But_Broke_back_end.ExternalApis.Yelp.Extensions
{
    public static class SearchResponseExtensions
    {
        public static List<Place> ToPlaces(this SearchResponse response)
        {
            if (response.Businesses is null) return [];

            return response.Businesses.Select(b =>
                new Place
                {
                    PlaceId = b.PlaceId ?? string.Empty,
                    PlaceName = b.PlaceName ?? string.Empty,
                    Location = new Location
                    {
                        Address1 = b.Location?.Address1 ?? string.Empty,
                        Address2 = b.Location?.Address2 ?? string.Empty,
                        Address3 = b.Location?.Address3 ?? string.Empty,
                        City = b.Location?.City ?? string.Empty,
                        ZipCode = b.Location?.ZipCode ?? string.Empty,
                        Country = b.Location?.Country ?? string.Empty,
                        State = b.Location?.State ?? string.Empty,
                        DisplayAddress = b.Location?.DisplayAddress ?? []
                    },
                    Categories = b.Categories?.Select(c => new Category
                    {
                        Alias = c.Alias ?? string.Empty,
                        Title = c.Title ?? string.Empty,
                    }).ToList() ?? [],
                    Price = (Price?)b.Price?.Length,
                    Coordinates = new Coordinates
                    {
                        Latitude = b.Coordinates?.Latitude,
                        Longitude = b.Coordinates?.Longitude
                    },
                    OpeningHours = b.BusinessHours?.Select(bh => new OpeningHours
                    {
                        Hours = bh.Hours?.Select(h => new Hour
                        {
                            Day = h.Day,
                            Start = h.Start ?? string.Empty,
                            End = h.End ?? string.Empty,
                        }).ToList() ?? [],
                        HoursType = bh.HoursType ?? string.Empty,
                        IsOpenNow = bh.IsOpenNow
                    }).ToList() ?? [],
                    Rating = b.Rating,
                    PlaceUrl = b.Attributes?.PlaceUrl ?? string.Empty,
                    ImageUrl = b.ImageUrl ?? string.Empty
                }
            ).ToList();
        }
        public static Place ToPlace(this YelpBusiness business)
        {
            return new Place
            {
                PlaceId = business.PlaceId ?? string.Empty,
                PlaceName = business.PlaceName ?? string.Empty,

                Location = new Location
                {
                    Address1 = business.Location?.Address1 ?? string.Empty,
                    Address2 = business.Location?.Address2 ?? string.Empty,
                    Address3 = business.Location?.Address3 ?? string.Empty,
                    City = business.Location?.City ?? string.Empty,
                    ZipCode = business.Location?.ZipCode ?? string.Empty,
                    Country = business.Location?.Country ?? string.Empty,
                    State = business.Location?.State ?? string.Empty,
                    DisplayAddress = business.Location?.DisplayAddress ?? []
                },

                Categories = business.Categories?.Select(c => new Category
                {
                    Alias = c.Alias ?? string.Empty,
                    Title = c.Title ?? string.Empty
                }).ToList() ?? [],

                Price = (Price?)business.Price?.Length,

                Coordinates = new Coordinates
                {
                    Latitude = business.Coordinates?.Latitude,
                    Longitude = business.Coordinates?.Longitude
                },

                OpeningHours = business.BusinessHours?.Select(bh => new OpeningHours
                {
                    Hours = bh.Hours?.Select(h => new Hour
                    {
                        Day = h.Day,
                        Start = h.Start ?? string.Empty,
                        End = h.End ?? string.Empty
                    }).ToList() ?? [],
                    HoursType = bh.HoursType ?? string.Empty,
                    IsOpenNow = bh.IsOpenNow
                }).ToList() ?? [],

                Rating = business.Rating,
                PlaceUrl = business.Attributes?.PlaceUrl ?? string.Empty,
                ImageUrl = business.ImageUrl ?? string.Empty
            };

           
        }
    }
}
