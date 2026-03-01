using BikeIotWebServer.Models;

namespace BikeIotWebServer.Infra
{
    public interface IBikeRepository
    {
        Task AddBikeAsync(Bike bike);
        IQueryable<Bike> GetAllBikesAsync();
    }
}
