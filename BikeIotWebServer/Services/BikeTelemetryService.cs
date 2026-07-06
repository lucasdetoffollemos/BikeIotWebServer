using BikeIotWebServer.Infra;
using BikeIotWebServer.Models;
using BikeIotWebServer.ViewModels;

namespace BikeIotWebServer.Services
{
    public class BikeTelemetryService
    {
        private readonly IBikeRepository _bikeRepository;

        public BikeTelemetryService(IBikeRepository bikeRepository)
        {
            _bikeRepository = bikeRepository;
        }

        public async Task SaveAsync(BikeTelemetry data)
        {
            var bikeData = new Bike
            {
                BikeId = data.BikeId,
                Speed = data.Velocidade,
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                Timestamp = data.Timestamp
            };

            await _bikeRepository.AddBikeAsync(bikeData);
        }
    }
}
