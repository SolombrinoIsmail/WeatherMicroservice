using WeatherMicroservice.Core.Enums;

namespace WeatherMicroservice.Core.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        public required Station Station { get; set; }
        public required DateTime Timestamp { get; set; }
        public required MeasurementType Type { get; set; }
        public required double Value { get; set; }
        public required string Unit { get; set; }
    }
}
