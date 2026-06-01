using Bored_But_Broke_back_end.ExternalApis.Geoapify;
using Bored_But_Broke_back_end.ExternalApis.Geoapify.Extensions;
using Bored_But_Broke_back_end.Models;

namespace Bored_But_Broke_back_end.Services
{
    public interface ILocationService
    {
        Task<Coordinates?> GetCoordinatesFromAddressAsync(string address, CancellationToken ct);
    }
    public class LocationService : ILocationService
    {
        private readonly IGeoapifyClient _geoapifyClient;
        public LocationService(IGeoapifyClient geoapifyClient)
        {
            _geoapifyClient = geoapifyClient;
        }

        public async Task<Coordinates?> GetCoordinatesFromAddressAsync(string address, CancellationToken token)
        {
            var response = await _geoapifyClient.ForwardGeocodingAsync(address, token);

            if (response.Results is [] || response.Results?[0]?.CountryCode != "gb")
            {
                return null;
            }

            return response.ToCoordinates();
        }
    }

}
