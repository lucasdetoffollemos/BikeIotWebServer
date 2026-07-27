using BikeIotWebServer.Infra;
using BikeIotWebServer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BikeIotWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikeLockController : ControllerBase
    {
        private readonly IBikeLockRepository _bikeLockRepository;

        public BikeLockController(IBikeLockRepository bikeLockRepository)
        {
            _bikeLockRepository = bikeLockRepository;
        }

        [HttpGet("{bikeId:int}")]
        public async Task<IActionResult> GetByBikeId([FromRoute] int bikeId)
        {
            var bikeLock = await _bikeLockRepository.GetByBikeIdAsync(bikeId);

            if (bikeLock == null)
                return NotFound();

            return Ok(new
            {
                bikeLock.BikeId,
                bikeLock.IsLock
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] BikeLockUpdateRequest request)
        {
            if (request == null)
                return BadRequest("Payload is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bikeLock = await _bikeLockRepository.UpsertAsync(request.BikeId, request.IsLock);

            return Ok(new
            {
                bikeLock.Id,
                bikeLock.BikeId,
                bikeLock.IsLock
            });
        }
    }
}
