using WeatherMicroservice.Core.Entities;
using WeatherMicroservice.Core.Enums;

namespace WeatherMicroservice.Core.Interfaces
{
    public interface IWeatherRepository
    {
        Task SaveMeasurements(List<Measurement> measurements);
        Task<List<Measurement>> GetMeasurements();
        Task<Measurement?> GetHighestMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null);
        Task<Measurement?> GetLowestMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null);
        Task<double> GetAverageMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null);
        Task<int> GetMeasurementCount(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null);
        Task<List<Measurement>> GetAllMeasurements(DateTime startDate, DateTime endDate, Station? station = null);
    }
}
