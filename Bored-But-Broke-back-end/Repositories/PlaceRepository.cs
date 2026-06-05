using Bored_But_Broke_back_end.Data;
using Bored_But_Broke_back_end.Models;

namespace Bored_But_Broke_back_end.Repositories
{
    public interface IPlaceRepository
    {
        Task<Place> UpsertAsync(Place place);
    }
    public class PlaceRepository : IPlaceRepository
    {
        private readonly AppDbContext _dbContext;

        public PlaceRepository(AppDbContext db)
        {
            _dbContext = db;
        }
        public async Task<Place> UpsertAsync(Place place)
        {
            var existing = await _dbContext.Places.FindAsync(place.PlaceId);

            if (existing is null)
            {
                _dbContext.Places.Add(place);
                await _dbContext.SaveChangesAsync();
                return place;
            }

            return existing;
        }
    }
}
