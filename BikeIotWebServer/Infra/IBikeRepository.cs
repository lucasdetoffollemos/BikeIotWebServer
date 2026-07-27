using BikeIotWebServer.Models;
using BikeIotWebServer.ViewModels;

namespace BikeIotWebServer.Infra
{
    public interface IBikeRepository
    {
        Task AddBikeAsync(Bike bike);
        Task<IReadOnlyList<Bike>> GetTelemetryHistoryAsync(TelemetryHistoryQuery query, CancellationToken cancellationToken = default);
    }
}
