using BikeIotWebServer.Infra;
using BikeIotWebServer.mqtt;
using BikeIotWebServer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeIotWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikeLockController : ControllerBase
    {
        private readonly IBikeLockRepository _bikeLockRepository;
        private readonly MqttPublisherService _mqttPublisherService;

        public BikeLockController(IBikeLockRepository bikeLockRepository, MqttPublisherService mqttPublisherService)
        {
            _bikeLockRepository = bikeLockRepository;
            _mqttPublisherService = mqttPublisherService;
        }

        [HttpGet("{bikeId:int}")]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> Update([FromBody] BikeLockUpdateRequest request)
        {
            if (request == null)
                return BadRequest("Payload is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bikeLock = await _bikeLockRepository.UpsertAsync(request.BikeId, request.IsLock);

            await _mqttPublisherService.PublishBikeLockAsync(request.BikeId, request.IsLock, HttpContext.RequestAborted);

            return Ok(new
            {
                bikeLock.Id,
                bikeLock.BikeId,
                bikeLock.IsLock
            });
        }
    }
}
