namespace WeatherMicroservice.Infrastructure.Models
{
    public class MeasurementApiResponse
    {
        public bool Ok { get; set; }
        public int TotalCount { get; set; }
        public int RowCount { get; set; }
        public List<MeasurementResponse> Result { get; set; } = new List<MeasurementResponse>();
    }
}
