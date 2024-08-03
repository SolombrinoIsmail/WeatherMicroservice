using Newtonsoft.Json;

namespace WeatherMicroservice.Infrastructure.Models
{
    public class MeasurementData
    {
        [JsonProperty("timestamp_cet")]
        public TimestampData? TimestampCet { get; set; }

        [JsonProperty("air_temperature")]
        public MeasurementValue? AirTemperature { get; set; }

        [JsonProperty("water_temperature")]
        public MeasurementValue? WaterTemperature { get; set; }

        [JsonProperty("wind_gust_max_10min")]
        public MeasurementValue? WindGustMax10Min { get; set; }

        [JsonProperty("wind_speed_avg_10min")]
        public MeasurementValue? WindSpeedAvg10Min { get; set; }

        [JsonProperty("wind_force_avg_10min")]
        public MeasurementValue? WindForceAvg10Min { get; set; }

        [JsonProperty("wind_direction")]
        public MeasurementValue? WindDirection { get; set; }

        [JsonProperty("windchill")]
        public MeasurementValue? Windchill { get; set; }

        [JsonProperty("barometric_pressure_qfe")]
        public MeasurementValue? BarometricPressureQfe { get; set; }

        [JsonProperty("precipitation")]
        public MeasurementValue? Precipitation { get; set; }

        [JsonProperty("dew_point")]
        public MeasurementValue? DewPoint { get; set; }

        [JsonProperty("global_radiation")]
        public MeasurementValue? GlobalRadiation { get; set; }

        [JsonProperty("humidity")]
        public MeasurementValue? Humidity { get; set; }

        [JsonProperty("water_level")]
        public MeasurementValue? WaterLevel { get; set; }
    }
}
