using Newtonsoft.Json;
using WeatherMicroservice.Core.Entities;
using WeatherMicroservice.Core.Interfaces;
using WeatherMicroservice.Infrastructure.Models;

namespace WeatherMicroservice.Infrastructure.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IWeatherRepository _weatherRepository;

        public WeatherService(HttpClient httpClient, IWeatherRepository weatherRepository)
        {
            _httpClient = httpClient;
            _weatherRepository = weatherRepository;
        }

        public async Task FetchAndStoreWeatherData(string station, string startDate, string endDate, string sort = "timestamp_cet desc", int limit = 100, int offset = 0)
        {
            var url = $"https://tecdottir.herokuapp.com/measurements/{station}?startDate={startDate}&endDate={endDate}&sort={sort}&limit={limit}&offset={offset}";
            var response = await _httpClient.GetStringAsync(url);
            var data = JsonConvert.DeserializeObject<MeasurementApiResponse>(response);

            if (data?.Result != null)
            {
                var validMeasurements = data.Result
                    .Where(m => m.Values?.AirTemperature?.Status == "ok")
                    .Select(m => new Measurement
                    {
                        Station = m.Station,
                        Timestamp = DateTime.TryParse(m.Timestamp, out var timestamp) ? timestamp : DateTime.MinValue,
                        AirTemperature = m.Values?.AirTemperature?.Value,
                        WaterTemperature = m.Values?.WaterTemperature?.Value,
                        BarometricPressure = m.Values?.BarometricPressureQfe?.Value,
                        Humidity = m.Values?.Humidity?.Value
                    }).ToList();

                await _weatherRepository.SaveMeasurements(validMeasurements);
            }
        }
    }
}
