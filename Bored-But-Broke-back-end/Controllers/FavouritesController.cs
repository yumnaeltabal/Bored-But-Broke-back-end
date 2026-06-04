using Bored_But_Broke_back_end.Models.Requests;
using Bored_But_Broke_back_end.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bored_But_Broke_back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavouritesController : ControllerBase
    {
        private readonly IFavouriteService _favouriteService;
        public FavouritesController(IFavouriteService favouriteService)
        {
            _favouriteService = favouriteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaceService>>> GetFavouritedPlacesAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var places = await _favouriteService.GetFavouritedPlacesAsync(userId);

            return Ok(places);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavouriteAsync([FromBody] AddFavouriteRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _favouriteService.AddFavouriteAsync(userId, request);

            return Created();
        }

        [HttpDelete("{placeId:length(22)}")]
        public async Task<IActionResult> RemoveFavouriteAsync(string placeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _favouriteService.RemoveFavouriteAsync(userId, placeId);

            return NoContent();
        }
    }
}