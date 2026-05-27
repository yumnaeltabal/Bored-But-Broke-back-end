using Bored_But_Broke_back_end.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bored_But_Broke_back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlacesController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlacesAsync(CancellationToken token)
        {
            var result = await _placeService.GetPlacesAsync(token);

            return Ok(result);
        }
    }
}
