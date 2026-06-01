using Bored_But_Broke_back_end.ExternalApis.Geoapify.Responses;
using Bored_But_Broke_back_end.Models;

namespace Bored_But_Broke_back_end.ExternalApis.Geoapify.Extensions
{
    public static class GeocodeResponseExtensions
    {
        public static Coordinates ToCoordinates(this GeocodeResponse response)
        {
            return new Coordinates
            {
                Latitude = response.Results?[0]?.Latitude,
                Longitude = response.Results?[0]?.Longitude
            };
        }
    }
}