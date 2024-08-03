using Microsoft.EntityFrameworkCore;
using WeatherMicroservice.Core.Enums;
using WeatherMicroservice.Core.Models;

namespace WeatherMicroservice.Infrastructure.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
            : base(options)
        {
        }

        public DbSet<Measurement> Measurements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Measurement>(entity =>
            {
                entity.Property(e => e.Station)
                    .HasConversion(
                        v => v.ToString(),
                        v => (Station)Enum.Parse(typeof(Station), v));

                entity.Property(e => e.Type)
                    .HasConversion(
                        v => v.ToString(),
                        v => (MeasurementType)Enum.Parse(typeof(MeasurementType), v));
            });
        }
    }
}
