using Microsoft.AspNetCore.Mvc;
using WeatherMicroservice.Core.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using WeatherMicroservice.Core.Enums;

namespace WeatherMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            this.weatherService = weatherService;
        }

        [HttpPost("fetch-previous-day")]
        [SwaggerOperation(Summary = "Fetch weather data for the previous day from specified stations.")]
        public async Task<IActionResult> FetchPreviousDayData()
        {
            await weatherService.FetchAndStorePreviousDayData();
            return Ok();
        }

        [HttpGet("measurements")]
        [SwaggerOperation(Summary = "Get all stored measurements.")]
        public async Task<ActionResult> GetMeasurements(
            [FromQuery, Required, DataType(DataType.Date), SwaggerParameter("The start date for the data range in YYYY-MM-DD format.", Required = true)] DateTime startDate,
            [FromQuery, Required, DataType(DataType.Date), SwaggerParameter("The end date for the data range in YYYY-MM-DD format.", Required = true)] DateTime endDate,
            [FromQuery, SwaggerParameter("Optional station filter.")] Station? station = null)
        {
            var measurements = await weatherService.GetAllMeasurements(startDate, endDate, station);
            if (measurements == null || measurements.Count == 0)
            {
                return NotFound();
            }
            return Ok(measurements);
        }

        [HttpGet("measurements/highest")]
        [SwaggerOperation(
            Summary = "Get the highest measurement for the specified type within a date range.",
            Description = "Retrieves the highest recorded measurement of the specified type within the provided date range. Optionally, filter by station. If no station is provided, data from both stations (Tiefenbrunnen and Mythenquai) will be used."
        )]
        [SwaggerResponse(200, "The highest measurement was successfully retrieved.")]
        [SwaggerResponse(404, "No measurements found for the specified criteria.")]
        public async Task<ActionResult> GetHighestMeasurement(
            [FromQuery, Required, SwaggerParameter("The type of measurement.", Required = true)] MeasurementType type,
            [FromQuery, Required, DataType(DataType.Date), SwaggerParameter("The start date for the data range in YYYY-MM-DD format.", Required = true)] DateTime startDate,
            [FromQuery, Required, DataType(DataType.Date), SwaggerParameter("The end date for the data range in YYYY-MM-DD format.", Required = true)] DateTime endDate,
            [FromQuery, SwaggerParameter("Optional station filter.")] Station? station = null)
        {
            var highestMeasurement = await weatherService.GetHighestMeasurement(type, startDate, endDate, station);
            if (highestMeasurement == null)
            {
                return NotFound();
            }
            return Ok(highestMeasurement);
        }

        [HttpGet("measurements/lowest")]
        [SwaggerOperation(
            Summary = "Get the lowest measurement for the specified type within a date range.",
            Description = "Retrieves the lowest recorded measurement of the specified type within the provided date range. Optionally, filter by station. If no station is provided, data from both stations (Tiefenbrunnen and Mythenquai) will be used."
        )]
        [SwaggerResponse(200, "The lowest measurement was successfully retrieved.")]
        [SwaggerResponse(404, "No measurements found for the specified criteria.")]
        public async Task<ActionResult> GetLowestMeasurement(
            [FromQuery, Required, SwaggerParameter("The type of measurement.", Required = true)] MeasurementType type,
            [FromQuery, Required, DataType(DataType.Date), SwaggerParameter("The start date for the data range in YYYY-MM-DD format.", Required = true)] DateTime startDate,
            [FromQuery, Required, DataType(DataType.Date), SwaggerParameter("The end date for the data range in YYYY-MM-DD format.", Required = true)] DateTime endDate,
            [FromQuery, SwaggerParameter("Optional station filter.")] Station? station = null)
        {
            var lowestMeasurement = await weatherService.GetLowestMeasurement(type, startDate, endDate, station);
            if (lowestMeasurement == null)
            {
                return NotFound();
            }
            return Ok(lowestMeasurement);
        }

        [HttpGet("measurements/average")]
        [SwaggerOperation(
            Summary = "Get the average measurement for the specified type within a date range.",
            Description = "Retrieves the average recorded measurement of the specified type within the provided date range. Optionally, filter by station. If no station is provided, data from both stations (Tiefenbrunnen and Mythenquai) will be used."
        )]
        [SwaggerResponse(200, "The average measurement was successfully retrieved.")]
        [SwaggerResponse(404, "No measurements found for the specified criteria.")]
        public async Task<ActionResult> GetAverageMeasurement(
            [FromQuery, Required, SwaggerParameter("The type of measurement.", Required = true)] MeasurementType type,
            [FromQuery, Required, DataType(DataType.Date), SwaggerParameter("The start date for the data range in YYYY-MM-DD format.", Required = true)] DateTime startDate,
            [FromQuery, Required, DataType(DataType.Date), SwaggerParameter("The end date for the data range in YYYY-MM-DD format.", Required = true)] DateTime endDate,
            [FromQuery, SwaggerParameter("Optional station filter.")] Station? station = null)
        {
            var averageMeasurement = await weatherService.GetAverageMeasurement(type, startDate, endDate, station);
            if (averageMeasurement == 0)
            {
                return NotFound();
            }
            return Ok(averageMeasurement);
        }

        [HttpGet("measurements/count")]
        [SwaggerOperation(
            Summary = "Get the total count of stored measurements within a date range.",
            Description = "Retrieves the total count of measurements of the specified type within the provided date range. Optionally, filter by station. If no station is provided, data from both stations (Tiefenbrunnen and Mythenquai) will be used."
        )]
        [SwaggerResponse(200, "The total count of measurements was successfully retrieved.")]
        [SwaggerResponse(404, "No measurements found for the specified criteria.")]
        public async Task<ActionResult> GetMeasurementCount(
            [FromQuery, Required, SwaggerParameter("The type of measurement.", Required = true)] MeasurementType type,
            [FromQuery, Required, DataType(DataType.Date), SwaggerParameter("The start date for the data range in YYYY-MM-DD format.", Required = true)] DateTime startDate,
            [FromQuery, Required, DataType(DataType.Date), SwaggerParameter("The end date for the data range in YYYY-MM-DD format.", Required = true)] DateTime endDate,
            [FromQuery, SwaggerParameter("Optional station filter.")] Station? station = null)
        {
            var measurementCount = await weatherService.GetMeasurementCount(type, startDate, endDate, station);
            if (measurementCount == 0)
            {
                return NotFound();
            }
            return Ok(measurementCount);
        }
    }
}
