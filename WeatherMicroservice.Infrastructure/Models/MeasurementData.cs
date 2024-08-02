namespace WeatherMicroservice.Infrastructure.Models
{
    public class MeasurementData
    {
        public TimestampData? TimestampCet { get; set; }
        public MeasurementValue? AirTemperature { get; set; }
        public MeasurementValue? WaterTemperature { get; set; }
        public MeasurementValue? WindGustMax10Min { get; set; }
        public MeasurementValue? WindSpeedAvg10Min { get; set; }
        public MeasurementValue? WindForceAvg10Min { get; set; }
        public MeasurementValue? WindDirection { get; set; }
        public MeasurementValue? Windchill { get; set; }
        public MeasurementValue? BarometricPressureQfe { get; set; }
        public MeasurementValue? Precipitation { get; set; }
        public MeasurementValue? DewPoint { get; set; }
        public MeasurementValue? GlobalRadiation { get; set; }
        public MeasurementValue? Humidity { get; set; }
        public MeasurementValue? WaterLevel { get; set; }
    }
}
