using Bored_But_Broke_back_end.Exceptions;
using Bored_But_Broke_back_end.Models;
using Bored_But_Broke_back_end.Models.Requests;
using Bored_But_Broke_back_end.Repositories;

namespace Bored_But_Broke_back_end.Services
{
    public interface IFavouriteService
    {
        Task<IEnumerable<Place>> GetFavouritedPlacesAsync(string userId);
        Task AddFavouriteAsync(string userId, AddFavouriteRequest request);
        Task RemoveFavouriteAsync(string userId, string productId);
        Task<IEnumerable<bool>> GetFavouriteStatusAsync(string userId, IEnumerable<string> placeIds);
    }
    public class FavouriteService : IFavouriteService
    {
        private readonly IFavouriteRepository _favouriteRepository;
        private readonly IPlaceRepository _placeRepository;

        public FavouriteService(IFavouriteRepository favouriteRepository, IPlaceRepository placeRepository)
        {
            _favouriteRepository = favouriteRepository;
            _placeRepository = placeRepository;
        }
        public async Task<IEnumerable<Place>> GetFavouritedPlacesAsync(string userId)
        {
            return await _favouriteRepository.GetPlacesByUserIdAsync(userId);
        }
        public async Task AddFavouriteAsync(string userId, AddFavouriteRequest request)
        {
            var place = await _placeRepository.UpsertAsync(
                new Place
                {
                    PlaceId = request.PlaceId,
                    PlaceName = request.PlaceName,
                    Location = request.Location,
                    Categories = request.Categories,
                    Price = request.Price,
                    Coordinates = request.Coordinates,
                    OpeningHours = request.OpeningHours,
                    Rating = request.Rating,
                    PlaceUrl = request.PlaceUrl,
                    ImageUrl = request.ImageUrl
                }
            );

            if (await _favouriteRepository.ExistsAsync(userId, place.PlaceId))
                throw new PlaceAlreadyFavouritedException();

            await _favouriteRepository.AddFavouriteAsync(new Favourite
            {
                UserId = userId,
                PlaceId = place.PlaceId,
                CreatedAt = DateTime.UtcNow
            });
        }
        public async Task RemoveFavouriteAsync(string userId, string productId)
        {
            var bookmark = await _favouriteRepository.GetByUserIdAndPlaceIdAsync(userId, productId)
                ?? throw new FavouriteNotFoundException();

            await _favouriteRepository.DeleteFavouriteAsync(bookmark);
        }
        public async Task<IEnumerable<bool>> GetFavouriteStatusAsync(string userId, IEnumerable<string> placeIds)
        {
            var placeIdList = placeIds.ToList();

            var favouritedIds = await _favouriteRepository
                .GetFavouritedPlaceIdsByUserIdAsync(userId, placeIdList);

            var favouritedSet = favouritedIds.ToHashSet();

            return placeIdList.Select(id => favouritedSet.Contains(id));
        }
    }
}
