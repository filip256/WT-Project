using System.ComponentModel.DataAnnotations;

namespace PoliceMaps.Entities
{
    public class PoliceHotspot
    {
        [Key]
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Severity { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime FirstOcurrence { get; set; }
    }
}
