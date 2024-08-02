using WeatherMicroservice.Core.Entities;

namespace WeatherMicroservice.Core.Interfaces
{
    public interface IWeatherRepository
    {
        Task SaveMeasurements(List<Measurement> measurements);
        Task<List<Measurement>> GetMeasurements();
        Task<Measurement?> GetHighestMeasurement(string type);
        Task<Measurement?> GetLowestMeasurement(string type);
        Task<double> GetAverageMeasurement(string type);
        Task<int> GetMeasurementCount();
    }
}
