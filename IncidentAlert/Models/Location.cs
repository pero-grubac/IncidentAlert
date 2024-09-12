using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentAlert.Models
{
    [Table("location")]
    public class Location : BaseEntity<int>
    {
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<Incident> Incidents { get; set; } = [];
    }
}
