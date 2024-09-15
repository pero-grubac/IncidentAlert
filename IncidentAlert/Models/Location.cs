using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentAlert.Models
{
    [Table("location")]
    [Index(nameof(Name), IsUnique = true)]
    public class Location
    {
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<Incident> Incidents { get; set; } = [];
    }
}
