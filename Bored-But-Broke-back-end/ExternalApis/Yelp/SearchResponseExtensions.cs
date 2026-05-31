using Bored_But_Broke_back_end.Models;

namespace Bored_But_Broke_back_end.ExternalApis.Yelp
{
    public static class SearchResponseExtensions
    {
        public static List<Models.Place> ToPlaces(this SearchResponse response)
        {
            return response.Businesses.Select(b => 
                new Models.Place
                {
                    PlaceId = b.PlaceId,
                    PlaceName = b.PlaceName,
                    Location = new Models.Location
                    {
                        Address1 = b.Location.Address1,
                        Address2 = b.Location.Address2,
                        Address3 = b.Location.Address3,
                        City = b.Location.City,
                        ZipCode = b.Location.ZipCode,
                        Country = b.Location.Country,
                        State = b.Location.State,
                        DisplayAddress = b.Location.DisplayAddress
                    },
                    Categories = b.Categories.Select(c => new Models.Category
                    {
                        Alias = c.Alias,
                        Title = c.Title,
                    }).ToList(),
                    Price = (Price)b.Price.Length,
                    Coordinates = new Models.Coordinates
                    {
                        Latitude = b.Coordinates.Latitude,
                        Longitude = b.Coordinates.Longitude
                    },
                    OpeningHours = b.BusinessHours.Select(bh => new OpeningHours
                    {
                        Hours = bh.Hours.Select(h => new Models.Hour
                        {
                            Day = h.Day,
                            Start = h.Start,
                            End = h.End,
                        }).ToList(),
                        HoursType = bh.HoursType,
                        IsOpenNow = bh.IsOpenNow
                    }).ToList(),
                    Rating = b.Rating,
                    PlaceUrl = b.Attributes.PlaceUrl,
                    ImageUrl = b.ImageUrl
                }
            ).ToList();
        }
    }
}
