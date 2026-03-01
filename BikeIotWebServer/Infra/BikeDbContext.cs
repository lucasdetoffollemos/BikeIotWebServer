using BikeIotWebServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BikeIotWebServer.Infra
{
    public class BikeContext : DbContext
    {
        public BikeContext(DbContextOptions<BikeContext> options) : base(options)
        {
        }

        public DbSet<Bike> Bikes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply explicit configuration for Bike
            modelBuilder.ApplyConfiguration(new BikeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
