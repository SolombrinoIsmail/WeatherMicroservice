using Microsoft.AspNetCore.Mvc;
using WeatherMicroservice.Core.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace WeatherMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IWeatherRepository _weatherRepository;

        public WeatherController(IWeatherService weatherService, IWeatherRepository weatherRepository)
        {
            _weatherService = weatherService;
            _weatherRepository = weatherRepository;
        }

        /// <summary>
        /// Fetch weather data from the specified station within the given date range.
        /// </summary>
        /// <param name="station">The station to fetch data from.</param>
        /// <param name="startDate">The start date for the data range (YYYY-MM-DD).</param>
        /// <param name="endDate">The end date for the data range (YYYY-MM-DD).</param>
        /// <param name="sort">The sorting order of the data (default: timestamp_cet desc).</param>
        /// <param name="limit">The number of rows to return (default: 100).</param>
        /// <param name="offset">The number of rows to offset (default: 0).</param>
        /// <returns>An action result indicating success or failure.</returns>
        [HttpPost("fetch")]
        [SwaggerOperation(Summary = "Fetch weather data from the specified station within the given date range.")]
        public async Task<IActionResult> FetchWeatherData([FromQuery] string station, [FromQuery] string startDate, [FromQuery] string endDate, [FromQuery] string sort = "timestamp_cet desc", [FromQuery] int limit = 100, [FromQuery] int offset = 0)
        {
            await _weatherService.FetchAndStoreWeatherData(station, startDate, endDate, sort, limit, offset);
            return Ok();
        }

        /// <summary>
        /// Get all stored measurements.
        /// </summary>
        /// <returns>A list of all measurements.</returns>
        [HttpGet("measurements")]
        [SwaggerOperation(Summary = "Get all stored measurements.")]
        public async Task<ActionResult> GetMeasurements()
        {
            var measurements = await _weatherRepository.GetMeasurements();
            return Ok(measurements);
        }

        /// <summary>
        /// Get the highest measurement for the specified type.
        /// </summary>
        /// <param name="type">The type of measurement (e.g., air_temperature, water_temperature).</param>
        /// <returns>The highest measurement of the specified type.</returns>
        [HttpGet("measurements/highest/{type}")]
        [SwaggerOperation(Summary = "Get the highest measurement for the specified type.")]
        public async Task<ActionResult> GetHighestMeasurement(string type)
        {
            var measurement = await _weatherRepository.GetHighestMeasurement(type);
            if (measurement == null)
            {
                return NotFound();
            }
            return Ok(measurement);
        }

        /// <summary>
        /// Get the lowest measurement for the specified type.
        /// </summary>
        /// <param name="type">The type of measurement (e.g., air_temperature, water_temperature).</param>
        /// <returns>The lowest measurement of the specified type.</returns>
        [HttpGet("measurements/lowest/{type}")]
        [SwaggerOperation(Summary = "Get the lowest measurement for the specified type.")]
        public async Task<ActionResult> GetLowestMeasurement(string type)
        {
            var measurement = await _weatherRepository.GetLowestMeasurement(type);
            if (measurement == null)
            {
                return NotFound();
            }
            return Ok(measurement);
        }

        /// <summary>
        /// Get the average measurement for the specified type.
        /// </summary>
        /// <param name="type">The type of measurement (e.g., air_temperature, water_temperature).</param>
        /// <returns>The average measurement of the specified type.</returns>
        [HttpGet("measurements/average/{type}")]
        [SwaggerOperation(Summary = "Get the average measurement for the specified type.")]
        public async Task<ActionResult> GetAverageMeasurement(string type)
        {
            var average = await _weatherRepository.GetAverageMeasurement(type);
            return Ok(average);
        }

        /// <summary>
        /// Get the total count of stored measurements.
        /// </summary>
        /// <returns>The total count of stored measurements.</returns>
        [HttpGet("measurements/count")]
        [SwaggerOperation(Summary = "Get the total count of stored measurements.")]
        public async Task<ActionResult> GetMeasurementCount()
        {
            var count = await _weatherRepository.GetMeasurementCount();
            return Ok(count);
        }
    }
}
