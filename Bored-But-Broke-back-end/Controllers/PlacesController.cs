using Bored_But_Broke_back_end.Models.Queries;
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
        public async Task<IActionResult> GetPlacesAsync([FromQuery] GetPlacesQuery query, CancellationToken token)
        {
            if (query.StartTime > query.EndTime)
            {
                ModelState.AddModelError(nameof(query.EndTime), "The end time must be later than the start time");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => $"{e.Key}: {string.Join(", ", e.Value!.Errors.Select(x => x.ErrorMessage))}")
                    .ToList();

                return ValidationProblem(
                    detail: string.Join(" | ", errors),
                    modelStateDictionary: ModelState);
            }

            var result = await _placeService.GetPlacesAsync(query, token);

            return Ok(result);
        }
        [HttpGet("{placeId:length(22)}")]
        public async Task<IActionResult> GetPlacesByIdAsync(string placeId, CancellationToken token)
        {
            var result = await _placeService.GetPlaceByIdAsync(placeId, token);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
