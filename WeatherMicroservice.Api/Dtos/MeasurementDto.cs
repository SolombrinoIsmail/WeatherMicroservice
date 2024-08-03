using WeatherMicroservice.Core.Enums;

namespace WeatherMicroservice.Api.Dtos
{
    public class MeasurementDto
    {
        public int Id { get; set; }
        public Station Station { get; set; }
        public DateTime Timestamp { get; set; }
        public MeasurementType Type { get; set; }
        public double Value { get; set; }
        public string? Unit { get; set; }
    }
}
