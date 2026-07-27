using BikeIotWebServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeIotWebServer.Infra
{
    public class BikeLockRepository : IBikeLockRepository
    {
        private readonly BikeContext _context;

        public BikeLockRepository(BikeContext context)
        {
            _context = context;
        }

        public async Task<BikeLock?> GetByBikeIdAsync(int bikeId)
        {
            return await _context.BikeLocks.FirstOrDefaultAsync(b => b.BikeId == bikeId);
        }

        public async Task<BikeLock> UpsertAsync(int bikeId, bool isLock)
        {
            var bikeLock = await _context.BikeLocks.FirstOrDefaultAsync(b => b.BikeId == bikeId);

            if (bikeLock == null)
            {
                bikeLock = new BikeLock
                {
                    BikeId = bikeId,
                    IsLock = isLock
                };

                await _context.BikeLocks.AddAsync(bikeLock);
            }
            else
            {
                bikeLock.IsLock = isLock;
            }

            await _context.SaveChangesAsync();

            return bikeLock;
        }
    }
}
