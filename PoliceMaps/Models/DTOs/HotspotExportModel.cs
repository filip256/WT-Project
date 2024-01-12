using PoliceMaps.Entities;

namespace PoliceMaps.Models.DTOs
{
    public class HotspotExportModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Severity { get; set; }
        public string LastDate { get; set; }
        public string Type { get; set; }

        public HotspotExportModel(Hotspot hotspot)
        {
            Latitude = hotspot.Latitude;
            Longitude = hotspot.Longitude;
            Severity = hotspot.Severity;
            LastDate = (hotspot.LastUpdate + TimeSpan.FromHours(3)).ToString("dddd, dd MMMM yyyy HH:mm");
            Type = hotspot.Type;
        }
    }
}
