using System.Net;
using Moq;
using Moq.Protected;
using WeatherMicroservice.Core.Enums;
using WeatherMicroservice.Core.Interfaces;
using WeatherMicroservice.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using WeatherMicroservice.Core.Models;

namespace WeatherMicroservice.Tests.Services
{
    public class WeatherServiceTests
    {
        private readonly Mock<HttpMessageHandler> mockHttpMessageHandler;
        private readonly Mock<IWeatherRepository> mockWeatherRepository;
        private readonly Mock<ILogger<WeatherService>> mockLogger;
        private readonly Mock<IConfiguration> mockConfiguration;
        private readonly WeatherService weatherService;
        private readonly HttpClient httpClient;

        public WeatherServiceTests()
        {
            mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockWeatherRepository = new Mock<IWeatherRepository>();
            mockLogger = new Mock<ILogger<WeatherService>>();
            mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(config => config["WeatherApi:BaseUrl"])
                .Returns("https://tecdottir.herokuapp.com");

            httpClient = new HttpClient(mockHttpMessageHandler.Object);
            weatherService = new WeatherService(httpClient, mockWeatherRepository.Object, mockLogger.Object, mockConfiguration.Object);
        }

        [Fact]
        public async Task FetchAndStorePreviousDayData_ShouldFetchAndStoreData()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"ok\":true,\"total_count\":1,\"row_count\":1,\"result\":[{\"station\":\"tiefenbrunnen\",\"timestamp\":\"2024-07-31T21:50:00.000Z\",\"values\":{\"air_temperature\":{\"value\":23.2,\"unit\":\"°C\",\"status\":\"ok\"},\"water_temperature\":{\"value\":25.5,\"unit\":\"°C\",\"status\":\"ok\"},\"barometric_pressure_qfe\":{\"value\":1012,\"unit\":\"hPa\",\"status\":\"ok\"},\"humidity\":{\"value\":60,\"unit\":\"%\",\"status\":\"ok\"}}}]}")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act
            await weatherService.FetchAndStorePreviousDayData();

            // Assert
            mockWeatherRepository.Verify(repo => repo.SaveMeasurements(It.IsAny<List<Measurement>>()), Times.Exactly(2));
        }

        [Fact]
        public async Task FetchAndStoreWeatherData_ShouldFetchAndStoreValidData()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"ok\":true,\"total_count\":1,\"row_count\":1,\"result\":[{\"station\":\"tiefenbrunnen\",\"timestamp\":\"2024-07-31T21:50:00.000Z\",\"values\":{\"air_temperature\":{\"value\":23.2,\"unit\":\"°C\",\"status\":\"ok\"},\"water_temperature\":{\"value\":25.5,\"unit\":\"°C\",\"status\":\"ok\"},\"barometric_pressure_qfe\":{\"value\":1012,\"unit\":\"hPa\",\"status\":\"ok\"},\"humidity\":{\"value\":60,\"unit\":\"%\",\"status\":\"ok\"}}}]}")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act
            await weatherService.FetchAndStoreWeatherData(Station.Tiefenbrunnen, "2024-07-31", "2024-08-01");

            // Assert
            mockWeatherRepository.Verify(repo => repo.SaveMeasurements(It.IsAny<List<Measurement>>()), Times.Once);
        }

        [Fact]
        public async Task FetchAndStoreWeatherData_ShouldLogErrorOnException()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => weatherService.FetchAndStoreWeatherData(Station.Tiefenbrunnen, "2024-07-31", "2024-08-01"));
            mockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task FetchAndStoreWeatherData_ShouldValidateInputs()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"ok\":true,\"total_count\":1,\"row_count\":1,\"result\":[{\"station\":\"tiefenbrunnen\",\"timestamp\":\"2024-07-31T21:50:00.000Z\",\"values\":{\"air_temperature\":{\"value\":23.2,\"unit\":\"°C\",\"status\":\"ok\"}}}]}")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => weatherService.FetchAndStoreWeatherData(Station.Tiefenbrunnen, "", "2024-08-01"));
            await Assert.ThrowsAsync<ArgumentException>(() => weatherService.FetchAndStoreWeatherData(Station.Tiefenbrunnen, "invalid-date", "2024-08-01"));
            await Assert.ThrowsAsync<ArgumentException>(() => weatherService.FetchAndStoreWeatherData(Station.Tiefenbrunnen, "2024-07-31", "invalid-date"));
        }

        [Fact]
        public async Task GetHighestMeasurement_ShouldReturnHighestMeasurement()
        {
            // Arrange
            var expectedMeasurement = new Measurement
            {
                Id = 1,
                Station = Station.Tiefenbrunnen,
                Timestamp = DateTime.UtcNow,
                Type = MeasurementType.AirTemperature,
                Value = 30.5,
                Unit = "°C"
            };

            mockWeatherRepository.Setup(repo => repo.GetHighestMeasurement(
                MeasurementType.AirTemperature, It.IsAny<DateTime>(), It.IsAny<DateTime>(), Station.Tiefenbrunnen))
                .ReturnsAsync(expectedMeasurement);

            // Act
            var actualMeasurement = await weatherService.GetHighestMeasurement(
                MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            Assert.NotNull(actualMeasurement);
            Assert.Equal(expectedMeasurement.Id, actualMeasurement.Id);
            Assert.Equal(expectedMeasurement.Station, actualMeasurement.Station);
            Assert.Equal(expectedMeasurement.Timestamp, actualMeasurement.Timestamp);
            Assert.Equal(expectedMeasurement.Type, actualMeasurement.Type);
            Assert.Equal(expectedMeasurement.Value, actualMeasurement.Value);
            Assert.Equal(expectedMeasurement.Unit, actualMeasurement.Unit);
        }

        [Fact]
        public async Task GetLowestMeasurement_ShouldReturnLowestMeasurement()
        {
            // Arrange
            var expectedMeasurement = new Measurement
            {
                Id = 1,
                Station = Station.Tiefenbrunnen,
                Timestamp = DateTime.UtcNow,
                Type = MeasurementType.AirTemperature,
                Value = 10.5,
                Unit = "°C"
            };

            mockWeatherRepository.Setup(repo => repo.GetLowestMeasurement(
                MeasurementType.AirTemperature, It.IsAny<DateTime>(), It.IsAny<DateTime>(), Station.Tiefenbrunnen))
                .ReturnsAsync(expectedMeasurement);

            // Act
            var actualMeasurement = await weatherService.GetLowestMeasurement(
                MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            Assert.NotNull(actualMeasurement);
            Assert.Equal(expectedMeasurement.Id, actualMeasurement.Id);
            Assert.Equal(expectedMeasurement.Station, actualMeasurement.Station);
            Assert.Equal(expectedMeasurement.Timestamp, actualMeasurement.Timestamp);
            Assert.Equal(expectedMeasurement.Type, actualMeasurement.Type);
            Assert.Equal(expectedMeasurement.Value, actualMeasurement.Value);
            Assert.Equal(expectedMeasurement.Unit, actualMeasurement.Unit);
        }

        [Fact]
        public async Task GetAverageMeasurement_ShouldReturnAverageMeasurement()
        {
            // Arrange
            var averageValue = 20.5;

            mockWeatherRepository.Setup(repo => repo.GetAverageMeasurement(MeasurementType.AirTemperature, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(averageValue);

            // Act
            var result = await weatherService.GetAverageMeasurement(MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            Assert.Equal(averageValue, result);
        }

        [Fact]
        public async Task GetMeasurementCount_ShouldReturnMeasurementCount()
        {
            // Arrange
            var count = 10;

            mockWeatherRepository.Setup(repo => repo.GetMeasurementCount(MeasurementType.AirTemperature, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(count);

            // Act
            var result = await weatherService.GetMeasurementCount(MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            Assert.Equal(count, result);
        }

        [Fact]
        public async Task GetAllMeasurements_ShouldReturnAllMeasurements()
        {
            // Arrange
            var measurements = new List<Measurement>
            {
                new() {
                    Station = Station.Tiefenbrunnen,
                    Timestamp = DateTime.UtcNow,
                    Type = MeasurementType.AirTemperature,
                    Value = 20.5,
                    Unit = "°C"
                },
                new() {
                    Station = Station.Mythenquai,
                    Timestamp = DateTime.UtcNow,
                    Type = MeasurementType.WaterTemperature,
                    Value = 18.3,
                    Unit = "°C"
                }
            };

            mockWeatherRepository.Setup(repo => repo.GetAllMeasurements(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(measurements);

            // Act
            var result = await weatherService.GetAllMeasurements(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            Assert.Equal(measurements.Count, result.Count);
            Assert.Equal(measurements[0].Id, result[0].Id);
            Assert.Equal(measurements[1].Id, result[1].Id);
        }
    }
}
