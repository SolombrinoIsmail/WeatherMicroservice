using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using WeatherMicroservice.Api.Controllers;
using WeatherMicroservice.Core.Entities;
using WeatherMicroservice.Core.Enums;
using WeatherMicroservice.Core.Interfaces;

namespace WeatherMicroservice.Tests.Controllers
{
    public class WeatherControllerTests
    {
        private readonly Mock<IWeatherService> _mockWeatherService;

        public WeatherControllerTests()
        {
            _mockWeatherService = new Mock<IWeatherService>();
        }

        [Fact]
        public async Task FetchPreviousDayData_ReturnsOk()
        {
            // Arrange
            _mockWeatherService.Setup(service => service.FetchAndStorePreviousDayData())
                .Returns(Task.CompletedTask);

            var controller = new WeatherController(_mockWeatherService.Object);

            // Act
            var result = await controller.FetchPreviousDayData();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetMeasurements_ReturnsOk()
        {
            // Arrange
            var measurements = new List<Measurement>
            {
                new Measurement
                {
                    Id = 1,
                    Station = Station.Tiefenbrunnen,
                    Timestamp = DateTime.UtcNow,
                    Type = MeasurementType.AirTemperature,
                    Value = 23.5,
                    Unit = "°C"
                }
            };
            _mockWeatherService.Setup(service => service.GetAllMeasurements(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Station?>()))
                .ReturnsAsync(measurements);

            var controller = new WeatherController(_mockWeatherService.Object);

            // Act
            var result = await controller.GetMeasurements(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.Equal(measurements, okResult.Value);
        }

        [Fact]
        public async Task GetMeasurements_ReturnsNotFound()
        {
            // Arrange
            _mockWeatherService.Setup(service => service.GetAllMeasurements(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Station?>()))
                .ReturnsAsync(new List<Measurement>());

            var controller = new WeatherController(_mockWeatherService.Object);

            // Act
            var result = await controller.GetMeasurements(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetHighestMeasurement_ReturnsOk()
        {
            // Arrange
            var measurement = new Measurement
            {
                Id = 1,
                Station = Station.Tiefenbrunnen,
                Timestamp = DateTime.UtcNow,
                Type = MeasurementType.AirTemperature,
                Value = 30.5,
                Unit = "°C"
            };
            _mockWeatherService.Setup(service => service.GetHighestMeasurement(It.IsAny<MeasurementType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Station?>()))
                .ReturnsAsync(measurement);

            var controller = new WeatherController(_mockWeatherService.Object);

            // Act
            var result = await controller.GetHighestMeasurement(MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.Equal(measurement, okResult.Value);
        }

        [Fact]
        public async Task GetHighestMeasurement_ReturnsNotFound()
        {
            // Arrange
            _mockWeatherService.Setup(service => service.GetHighestMeasurement(It.IsAny<MeasurementType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Station?>()))
                .ReturnsAsync((Measurement?)null);

            var controller = new WeatherController(_mockWeatherService.Object);

            // Act
            var result = await controller.GetHighestMeasurement(MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetLowestMeasurement_ReturnsOk()
        {
            // Arrange
            var measurement = new Measurement
            {
                Id = 1,
                Station = Station.Tiefenbrunnen,
                Timestamp = DateTime.UtcNow,
                Type = MeasurementType.AirTemperature,
                Value = 10.5,
                Unit = "°C"
            };
            _mockWeatherService.Setup(service => service.GetLowestMeasurement(It.IsAny<MeasurementType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Station?>()))
                .ReturnsAsync(measurement);

            var controller = new WeatherController(_mockWeatherService.Object);

            // Act
            var result = await controller.GetLowestMeasurement(MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.Equal(measurement, okResult.Value);
        }

        [Fact]
        public async Task GetLowestMeasurement_ReturnsNotFound()
        {
            // Arrange
            _mockWeatherService.Setup(service => service.GetLowestMeasurement(It.IsAny<MeasurementType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Station?>()))
                .ReturnsAsync((Measurement?)null);

            var controller = new WeatherController(_mockWeatherService.Object);

            // Act
            var result = await controller.GetLowestMeasurement(MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetAverageMeasurement_ReturnsOk()
        {
            // Arrange
            double averageValue = 20.5;
            _mockWeatherService.Setup(service => service.GetAverageMeasurement(It.IsAny<MeasurementType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Station?>()))
                .ReturnsAsync(averageValue);

            var controller = new WeatherController(_mockWeatherService.Object);

            // Act
            var result = await controller.GetAverageMeasurement(MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.Equal(averageValue, okResult.Value);
        }

        [Fact]
        public async Task GetMeasurementCount_ReturnsOk()
        {
            // Arrange
            int count = 10;
            _mockWeatherService.Setup(service => service.GetMeasurementCount(It.IsAny<MeasurementType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Station?>()))
                .ReturnsAsync(count);

            var controller = new WeatherController(_mockWeatherService.Object);

            // Act
            var result = await controller.GetMeasurementCount(MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.Equal(count, okResult.Value);
        }

        [Fact]
        public async Task GetMeasurementCount_ReturnsNotFound()
        {
            // Arrange
            _mockWeatherService.Setup(service => service.GetMeasurementCount(It.IsAny<MeasurementType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Station?>()))
                .ReturnsAsync(0);

            var controller = new WeatherController(_mockWeatherService.Object);

            // Act
            var result = await controller.GetMeasurementCount(MeasurementType.AirTemperature, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }
    }
}
