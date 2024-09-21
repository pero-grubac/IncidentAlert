using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentAlert.Models
{
    [Table("incident")]
    public class Incident
    {
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;


        [ForeignKey("LocationId")]
        public int LocationId { get; set; }
        public Location? Location { get; set; }

        public ICollection<IncidentCategory> IncidentCategories { get; set; } = new List<IncidentCategory>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
