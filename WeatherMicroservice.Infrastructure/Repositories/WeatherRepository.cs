using WeatherMicroservice.Core.Interfaces;
using WeatherMicroservice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using WeatherMicroservice.Core.Enums;
using WeatherMicroservice.Core.Models;

namespace WeatherMicroservice.Infrastructure.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly WeatherDbContext context;

        public WeatherRepository(WeatherDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task SaveMeasurements(List<Measurement> measurements)
        {
            foreach (var measurement in measurements)
            {
                // Check if the measurement already exists to avoid duplicates
                var existingMeasurement = await context.Measurements
                    .FirstOrDefaultAsync(m => m.Timestamp == measurement.Timestamp && m.Station == measurement.Station && m.Type == measurement.Type);

                if (existingMeasurement == null)
                {
                    await context.Measurements.AddAsync(measurement);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<Measurement>> GetMeasurements()
        {
            return await context.Measurements.ToListAsync();
        }

        public async Task<Measurement?> GetHighestMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null)
        {
            var query = context.Measurements.Where(m => m.Type == type && m.Timestamp >= startDate && m.Timestamp <= endDate);

            if (station != null)
            {
                query = query.Where(m => m.Station == station);
            }

            return await query.OrderByDescending(m => m.Value).FirstOrDefaultAsync();
        }

        public async Task<Measurement?> GetLowestMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null)
        {
            var query = context.Measurements.Where(m => m.Type == type && m.Timestamp >= startDate && m.Timestamp <= endDate);

            if (station != null)
            {
                query = query.Where(m => m.Station == station);
            }

            return await query.OrderBy(m => m.Value).FirstOrDefaultAsync();
        }

        public async Task<double> GetAverageMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null)
        {
            var query = context.Measurements.Where(m => m.Type == type && m.Timestamp >= startDate && m.Timestamp <= endDate);

            if (station != null)
            {
                query = query.Where(m => m.Station == station);
            }

            return await query.AverageAsync(m => m.Value);
        }

        public async Task<int> GetMeasurementCount(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null)
        {
            var query = context.Measurements.Where(m => m.Type == type && m.Timestamp >= startDate && m.Timestamp <= endDate);

            if (station != null)
            {
                query = query.Where(m => m.Station == station);
            }

            return await query.CountAsync();
        }

        public async Task<List<Measurement>> GetAllMeasurements(DateTime startDate, DateTime endDate, Station? station = null)
        {
            var query = context.Measurements.Where(m => m.Timestamp >= startDate && m.Timestamp <= endDate);

            if (station != null)
            {
                query = query.Where(m => m.Station == station);
            }

            return await query.ToListAsync();
        }
    }
}
