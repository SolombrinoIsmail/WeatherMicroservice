using Newtonsoft.Json;

namespace WeatherMicroservice.Infrastructure.Models
{
    public class MeasurementApiResponse
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("row_count")]
        public int RowCount { get; set; }

        [JsonProperty("result")]
        public List<MeasurementResponse> Result { get; set; } = new List<MeasurementResponse>();
    }
}
