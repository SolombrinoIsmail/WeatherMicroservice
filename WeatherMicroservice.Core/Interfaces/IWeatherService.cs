using WeatherMicroservice.Core.Enums;
using WeatherMicroservice.Core.Models;

namespace WeatherMicroservice.Core.Interfaces
{
    public interface IWeatherService
    {
        Task FetchAndStorePreviousDayData();
        Task FetchAndStoreWeatherData(Station station, string startDate, string endDate, string sort = "timestamp_cet desc", int limit = 100, int offset = 0);
        Task<Measurement?> GetHighestMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null);
        Task<Measurement?> GetLowestMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null);
        Task<double> GetAverageMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null);
        Task<int> GetMeasurementCount(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null);
        Task<List<Measurement>> GetAllMeasurements(DateTime startDate, DateTime endDate, Station? station = null);

    }
}
