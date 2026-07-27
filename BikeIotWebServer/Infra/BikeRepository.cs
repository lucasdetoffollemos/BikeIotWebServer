using BikeIotWebServer.Models;
using BikeIotWebServer.ViewModels;
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

        public async Task<IReadOnlyList<Bike>> GetTelemetryHistoryAsync(TelemetryHistoryQuery query, CancellationToken cancellationToken = default)
        {
            var bikes = _context.Bikes.AsNoTracking().AsQueryable();

            if (query.BikeId.HasValue)
            {
                bikes = bikes.Where(b => b.BikeId == query.BikeId.Value);
            }

            if (query.From.HasValue)
            {
                bikes = bikes.Where(b => b.Timestamp >= query.From.Value);
            }

            if (query.To.HasValue)
            {
                bikes = bikes.Where(b => b.Timestamp <= query.To.Value);
            }

            bikes = string.Equals(query.Order, "asc", StringComparison.OrdinalIgnoreCase)
                ? bikes.OrderBy(b => b.Timestamp).ThenBy(b => b.Id)
                : bikes.OrderByDescending(b => b.Timestamp).ThenByDescending(b => b.Id);

            return await bikes
                .Skip(query.Offset)
                .Take(query.Limit)
                .ToListAsync(cancellationToken);
        }
    }
}
