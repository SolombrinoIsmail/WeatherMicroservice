using Microsoft.EntityFrameworkCore;
using WeatherMicroservice.Core.Entities;
using WeatherMicroservice.Core.Interfaces;
using WeatherMicroservice.Infrastructure.Data;

namespace WeatherMicroservice.Infrastructure.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly WeatherDbContext _context;

        public WeatherRepository(WeatherDbContext context)
        {
            _context = context;
        }

        public async Task SaveMeasurements(List<Measurement> measurements)
        {
            foreach (var measurement in measurements)
            {
                if (!_context.Measurements.Any(m => m.Timestamp == measurement.Timestamp && m.Station == measurement.Station))
                {
                    _context.Measurements.Add(measurement);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Measurement>> GetMeasurements()
        {
            return await _context.Measurements.ToListAsync();
        }

        public async Task<Measurement?> GetHighestMeasurement(string type)
        {
            return await _context.Measurements
                .OrderByDescending(m => EF.Property<double>(m, type))
                .FirstOrDefaultAsync();
        }

        public async Task<Measurement?> GetLowestMeasurement(string type)
        {
            return await _context.Measurements
                .OrderBy(m => EF.Property<double>(m, type))
                .FirstOrDefaultAsync();
        }

        public async Task<double> GetAverageMeasurement(string type)
        {
            return await _context.Measurements
                .AverageAsync(m => EF.Property<double>(m, type));
        }

        public async Task<int> GetMeasurementCount()
        {
            return await _context.Measurements.CountAsync();
        }
    }
}
