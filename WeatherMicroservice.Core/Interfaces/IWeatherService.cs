namespace WeatherMicroservice.Core.Interfaces
{
    public interface IWeatherService
    {
        Task FetchAndStoreWeatherData(string station, string startDate, string endDate, string sort = "timestamp_cet desc", int limit = 100, int offset = 0);
    }
}
