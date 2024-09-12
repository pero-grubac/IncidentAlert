using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentAlert.Models
{
    [Table("incident")]
    public class Incident : BaseEntity<int>
    {

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public DateTime DateTime { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; } = null!;

        [Required]
        public ICollection<IncidentCategory> IncidentCategories { get; set; } = [];
    }
}
