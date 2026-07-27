using BikeIotWebServer.Infra;
using BikeIotWebServer.Services;
using BikeIotWebServer.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public IActionResult GetStatus()
        {
            return Ok(true);
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> GetAllTelemetry([FromQuery] TelemetryHistoryQuery query, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (query.From.HasValue && query.To.HasValue && query.From > query.To)
                return BadRequest("The 'from' value must be earlier than or equal to 'to'.");

            var bikes = await _bikeRepository.GetTelemetryHistoryAsync(query, cancellationToken);

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
