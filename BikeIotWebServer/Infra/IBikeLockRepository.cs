using BikeIotWebServer.Models;

namespace BikeIotWebServer.Infra
{
    public interface IBikeLockRepository
    {
        Task<BikeLock?> GetByBikeIdAsync(int bikeId);
        Task<BikeLock> UpsertAsync(int bikeId, bool isLock);
    }
}
