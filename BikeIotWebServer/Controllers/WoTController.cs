using BikeIotWebServer.WoT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeIotWebServer.Controllers
{
    [ApiController]
    public class WoTController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public WoTController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("td")]
        [AllowAnonymous]
        public IActionResult GetThingDescription()
        {
            var httpBaseUrl = GetHttpBaseUrl();
            var mqttHost = _configuration["Mqtt:Host"] ?? "localhost";
            var mqttPort = _configuration.GetValue<int?>("Mqtt:Port") ?? 1883;
            var td = WotDescriptionFactory.BuildThingDescription(httpBaseUrl, mqttHost, mqttPort);

            return Content(td, "application/td+json");
        }

        [HttpGet(".well-known/wot")]
        [AllowAnonymous]
        public IActionResult GetDiscoveryDocument()
        {
            var discovery = WotDescriptionFactory.BuildDiscoveryDocument(GetHttpBaseUrl());
            return Content(discovery, "application/json");
        }

        private string GetHttpBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}";
        }
    }
}
