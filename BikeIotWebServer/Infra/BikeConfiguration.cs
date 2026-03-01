using BikeIotWebServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BikeIotWebServer.Infra
{
    public class BikeConfiguration : IEntityTypeConfiguration<Bike>
    {
        public void Configure(EntityTypeBuilder<Bike> builder)
        {
            // Primary key
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                   .IsRequired()
                   .ValueGeneratedOnAdd();

            // Basic properties
            builder.Property(b => b.Speed)
                   .IsRequired();

            builder.Property(b => b.Latitude)
                   .IsRequired();

            builder.Property(b => b.Longitude)
                   .IsRequired();

            builder.Property(b => b.Timestamp)
                   .IsRequired();
        }
    }
}
