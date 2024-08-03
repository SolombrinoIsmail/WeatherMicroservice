using Newtonsoft.Json;

namespace WeatherMicroservice.Infrastructure.Models
{
    public class MeasurementValue
    {
        [JsonProperty("value")]
        public double? Value { get; set; }

        [JsonProperty("unit")]
        public string? Unit { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }
    }
}
