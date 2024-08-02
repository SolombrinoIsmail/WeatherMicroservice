namespace WeatherMicroservice.Core.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public string? Station { get; set; }
        public DateTime Timestamp { get; set; }
        public double? AirTemperature { get; set; }
        public double? WaterTemperature { get; set; }
        public double? BarometricPressure { get; set; }
        public double? Humidity { get; set; }
    }
}
