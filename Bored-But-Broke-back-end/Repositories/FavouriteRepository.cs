using Bored_But_Broke_back_end.Data;
using Bored_But_Broke_back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace Bored_But_Broke_back_end.Repositories
{
    public interface IFavouriteRepository
    {
        Task<IEnumerable<Place>> GetPlacesByUserIdAsync(string userId);
        Task<bool> ExistsAsync(string userId, string placeId);
        Task AddFavouriteAsync(Favourite favourite);
        Task<Favourite?> GetByUserIdAndPlaceIdAsync(string userId, string placeId);
        Task<IEnumerable<string>> GetFavouritedPlaceIdsByUserIdAsync(string userId, IEnumerable<string> placeIds);
        Task DeleteFavouriteAsync(Favourite favourite);
    }
    public class FavouriteRepository : IFavouriteRepository
    {
        private readonly AppDbContext _dbContext;

        public FavouriteRepository(AppDbContext db)
        {
            _dbContext = db;
        }
        public async Task<IEnumerable<Place>> GetPlacesByUserIdAsync(string userId)
        {
            return await _dbContext.Favourites
                .Where(f => f.UserId == userId)
                .Include(f => f.Place)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => f.Place)
                .ToListAsync();
        }
        public async Task<bool> ExistsAsync(string userId, string placeId)
        {
            return await _dbContext.Favourites
                .AnyAsync(b => b.UserId == userId && b.PlaceId == placeId);
        }
        public async Task AddFavouriteAsync(Favourite favourite)
        {
            _dbContext.Favourites.Add(favourite);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Favourite?> GetByUserIdAndPlaceIdAsync(string userId, string placeId)
        {
            return await _dbContext.Favourites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PlaceId == placeId);
        }
        public async Task DeleteFavouriteAsync(Favourite favourite)
        {
            _dbContext.Favourites.Remove(favourite);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<string>> GetFavouritedPlaceIdsByUserIdAsync(string userId, IEnumerable<string> placeIds)
        {
            return await _dbContext.Favourites
                .Where(f => f.UserId == userId && placeIds.Contains(f.PlaceId))
                .Select(f => f.PlaceId)
                .ToListAsync();
        }
    }
}
