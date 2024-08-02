using Microsoft.EntityFrameworkCore;
using WeatherMicroservice.Core.Entities;

namespace WeatherMicroservice.Infrastructure.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
        {
        }

        public DbSet<Measurement> Measurements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add additional configuration if needed
        }
    }
}
