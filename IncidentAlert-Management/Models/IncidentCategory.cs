using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentAlert_Management.Models
{
    [Table("incident_category")]
    public class IncidentCategory
    {
        public int IncidentId { get; set; }

        public Incident Incident { get; set; } = null!;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

    }
}
