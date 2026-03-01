using BikeIotWebServer.Infra;
using BikeIotWebServer.Models;
using BikeIotWebServer.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BikeIotWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikeController : ControllerBase
    {
        public readonly IBikeRepository _bikeRepository;
        public BikeController(IBikeRepository bikeRepository)
        {
             _bikeRepository = bikeRepository;
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

            var bikeData = new Bike
            {
                Speed = data.Velocidade,
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                Timestamp = data.Timestamp
            };

            await _bikeRepository.AddBikeAsync(bikeData);

            return Ok(new
            {
                received = true,
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
                Velocidade = (float)b.Speed,
                Latitude = (float)b.Latitude,
                Longitude = (float)b.Longitude,
                Timestamp = b.Timestamp
            }).ToList();

            return Ok(telemetry);
        }
    }
}
