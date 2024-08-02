namespace WeatherMicroservice.Infrastructure.Models
{
    public class MeasurementResponse
    {
        public string? Station { get; set; }
        public string? Timestamp { get; set; }
        public MeasurementData Values { get; set; } = new MeasurementData();
    }
}
