using System.ComponentModel.DataAnnotations;

namespace PoliceMaps.Entities
{
    public class SurveyArea
    {
        [Key]
        public int Id { get; set; }
        public double StartLatitude { get; set; }
        public double StartLongitude { get; set; }
        public double EndLatitude { get; set; }
        public double EndLongitude { get; set; }
    }
}
