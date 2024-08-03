using Newtonsoft.Json;

namespace WeatherMicroservice.Infrastructure.Models
{
    public class MeasurementResponse
    {
        [JsonProperty("station")]
        public string? Station { get; set; }

        [JsonProperty("timestamp")]
        public string? Timestamp { get; set; }

        [JsonProperty("values")]
        public MeasurementData Values { get; set; } = new MeasurementData();
    }
}
