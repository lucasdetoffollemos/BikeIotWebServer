using BikeIotWebServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeIotWebServer.Infra
{
    public class BikeRepository : IBikeRepository
    {
        public readonly BikeContext _context;

        public BikeRepository(BikeContext context) 
        {
            _context = context;
        }

        public async Task AddBikeAsync(Bike bike)
        {
            await _context.Bikes.AddAsync(bike);

            await _context.SaveChangesAsync();
        }

        public IQueryable<Bike> GetAllBikesAsync()
        {
            return _context.Bikes.AsQueryable();
        }
    }
}
