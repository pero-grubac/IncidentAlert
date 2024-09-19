using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentAlert.Models
{
    [Table("images")]

    public class Image
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FilePath { get; set; } = string.Empty;

        [ForeignKey("IncidentId")]
        public int IncidentId { get; set; }

    }
}
