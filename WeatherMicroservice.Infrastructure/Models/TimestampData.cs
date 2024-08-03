using Newtonsoft.Json;

namespace WeatherMicroservice.Infrastructure.Models
{
    public class TimestampData
    {
        [JsonProperty("value")]
        public string? Value { get; set; }

        [JsonProperty("unit")]
        public string? Unit { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }
    }
}
