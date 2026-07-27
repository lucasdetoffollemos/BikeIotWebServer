using BikeIotWebServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeIotWebServer.Infra
{
    public class BikeLockConfiguration : IEntityTypeConfiguration<BikeLock>
    {
        public void Configure(EntityTypeBuilder<BikeLock> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                   .IsRequired()
                   .ValueGeneratedOnAdd();

            builder.Property(b => b.BikeId)
                   .IsRequired();

            builder.Property(b => b.IsLock)
                   .IsRequired();
        }
    }
}
