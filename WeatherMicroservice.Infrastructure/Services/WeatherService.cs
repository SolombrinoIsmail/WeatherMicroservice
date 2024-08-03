using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using WeatherMicroservice.Core.Entities;
using WeatherMicroservice.Core.Interfaces;
using WeatherMicroservice.Infrastructure.Models;
using WeatherMicroservice.Core.Enums;

namespace WeatherMicroservice.Infrastructure.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient httpClient;
        private readonly IWeatherRepository weatherRepository;
        private readonly ILogger<WeatherService> logger;
        private readonly IConfiguration configuration;

        public WeatherService(HttpClient httpClient, IWeatherRepository weatherRepository, ILogger<WeatherService> logger, IConfiguration configuration)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.weatherRepository = weatherRepository ?? throw new ArgumentNullException(nameof(weatherRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task FetchAndStorePreviousDayData()
        {
            var stations = new List<Station> { Station.Tiefenbrunnen, Station.Mythenquai };
            var startDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
            var endDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            foreach (var station in stations)
            {
                await FetchAndStoreWeatherData(station, startDate, endDate);
            }
        }

        public async Task FetchAndStoreWeatherData(Station station, string startDate, string endDate, string sort = "timestamp_cet desc", int limit = 100, int offset = 0)
        {
            try
            {
                ValidateInputs(startDate, endDate, limit, offset);

                var url = ConstructUrl(station, startDate, endDate, sort, limit, offset);
                logger.LogInformation($"Requesting data from URL: {url}");

                var response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<MeasurementApiResponse>(responseString);

                if (data?.Result != null)
                {
                    var validMeasurements = ExtractValidMeasurements(data.Result);
                    await weatherRepository.SaveMeasurements(validMeasurements);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching and storing weather data.");
                throw;
            }
        }

        public async Task<Measurement?> GetHighestMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null)
        {
            if (station.HasValue)
            {
                return await weatherRepository.GetHighestMeasurement(type, startDate, endDate, station.Value);
            }

            var stations = Enum.GetValues(typeof(Station)).Cast<Station>();
            Measurement? highestMeasurement = null;

            foreach (var st in stations)
            {
                var measurement = await weatherRepository.GetHighestMeasurement(type, startDate, endDate, st);
                if (measurement != null && (highestMeasurement == null || measurement.Value > highestMeasurement.Value))
                {
                    highestMeasurement = measurement;
                }
            }

            return highestMeasurement;
        }

        public async Task<Measurement?> GetLowestMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null)
        {
            if (station.HasValue)
            {
                return await weatherRepository.GetLowestMeasurement(type, startDate, endDate, station.Value);
            }

            var stations = Enum.GetValues(typeof(Station)).Cast<Station>();
            Measurement? lowestMeasurement = null;

            foreach (var st in stations)
            {
                var measurement = await weatherRepository.GetLowestMeasurement(type, startDate, endDate, st);
                if (measurement != null && (lowestMeasurement == null || measurement.Value < lowestMeasurement.Value))
                {
                    lowestMeasurement = measurement;
                }
            }

            return lowestMeasurement;
        }

        public async Task<double> GetAverageMeasurement(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null)
        {
            return await weatherRepository.GetAverageMeasurement(type, startDate, endDate, station);
        }

        public async Task<int> GetMeasurementCount(MeasurementType type, DateTime startDate, DateTime endDate, Station? station = null)
        {
            return await weatherRepository.GetMeasurementCount(type, startDate, endDate, station);
        }

        public async Task<List<Measurement>> GetAllMeasurements(DateTime startDate, DateTime endDate, Station? station = null)
        {
            return await weatherRepository.GetAllMeasurements(startDate, endDate, station);
        }

        private void ValidateInputs(string startDate, string endDate, int limit, int offset)
        {
            if (!DateTime.TryParse(startDate, out _))
                throw new ArgumentException("Invalid start date format.", nameof(startDate));

            if (!DateTime.TryParse(endDate, out _))
                throw new ArgumentException("Invalid end date format.", nameof(endDate));

            if (limit <= 0)
                throw new ArgumentException("Limit must be greater than 0.", nameof(limit));

            if (limit > 1000)
                throw new ArgumentException("Limit must be smaller than 1000.", nameof(limit));

            if (offset < 0)
                throw new ArgumentException("Offset cannot be negative.", nameof(offset));
        }

        private string ConstructUrl(Station station, string startDate, string endDate, string sort, int limit, int offset)
        {
            var baseUrl = configuration["WeatherApi:BaseUrl"];
            var stationString = station.ToString().ToLower();
            return $"{baseUrl}/measurements/{stationString}?startDate={startDate}&endDate={endDate}&sort={sort}&limit={limit}&offset={offset}";
        }

        private List<Measurement> ExtractValidMeasurements(List<MeasurementResponse> responses)
        {
            var validMeasurements = new List<Measurement>();

            foreach (var result in responses)
            {
                AddValidMeasurement(validMeasurements, result, MeasurementType.AirTemperature, result.Values?.AirTemperature);
                AddValidMeasurement(validMeasurements, result, MeasurementType.WaterTemperature, result.Values?.WaterTemperature);
                AddValidMeasurement(validMeasurements, result, MeasurementType.BarometricPressure, result.Values?.BarometricPressureQfe);
                AddValidMeasurement(validMeasurements, result, MeasurementType.Humidity, result.Values?.Humidity);
            }

            return validMeasurements;
        }

        private void AddValidMeasurement(List<Measurement> measurements, MeasurementResponse result, MeasurementType type, MeasurementValue? value)
        {
            if (value != null && value.Status == "ok" &&
                !string.IsNullOrEmpty(result.Station) &&
                !string.IsNullOrEmpty(result.Timestamp) &&
                value.Value.HasValue &&
                !string.IsNullOrEmpty(value.Unit))
            {
                if (Enum.TryParse<Station>(result.Station, true, out var station))
                {
                    measurements.Add(new Measurement
                    {
                        Station = station,
                        Timestamp = DateTime.TryParse(result.Timestamp, out var timestamp) ? timestamp : DateTime.MinValue,
                        Type = type,
                        Value = value.Value.Value,
                        Unit = value.Unit
                    });
                }
                else
                {
                    logger.LogWarning($"Unknown station: {result.Station}");
                }
            }
        }
    }
}
