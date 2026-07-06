using BikeIotWebServer.Infra;
using BikeIotWebServer.Services;
using BikeIotWebServer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BikeIotWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikeController : ControllerBase
    {
        public readonly IBikeRepository _bikeRepository;
        private readonly BikeTelemetryService _bikeTelemetryService;

        public BikeController(IBikeRepository bikeRepository, BikeTelemetryService bikeTelemetryService)
        {
             _bikeRepository = bikeRepository;
             _bikeTelemetryService = bikeTelemetryService;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> PostData([FromBody] BikeTelemetry data)
        {
            if (data == null)
                return BadRequest("Payload is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _bikeTelemetryService.SaveAsync(data);

            return Ok(new
            {
                received = true,
                bikeId = data.BikeId,
                velocidade = data.Velocidade,
                posicao = new { data.Latitude, data.Longitude },
                timestamp = data.Timestamp
            });
        }

        // GET api/bike
        [HttpGet]
        public IActionResult GetAllTelemetry()
        {
            var bikes = _bikeRepository.GetAllBikesAsync();

            var telemetry = bikes.Select(b => new BikeTelemetry
            {
                BikeId = b.BikeId,
                Velocidade = (float)b.Speed,
                Latitude = (float)b.Latitude,
                Longitude = (float)b.Longitude,
                Timestamp = b.Timestamp
            }).ToList();

            return Ok(telemetry);
        }
    }
}
