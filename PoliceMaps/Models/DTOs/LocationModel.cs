namespace PoliceMaps.Models.DTOs
{
    public class LocationModel
    {
        public string ExternalId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Type { get; set; }
        public int Confidence { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
