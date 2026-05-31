using Bored_But_Broke_back_end.Models;
using Bored_But_Broke_back_end.Models.Queries;

namespace Bored_But_Broke_back_end.Services
{
    public interface IPlaceService
    {
        Task<List<Place>> GetPlacesAsync(GetPlacesQuery query, CancellationToken ct);
    }
}
