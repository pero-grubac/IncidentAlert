using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace IncidentAlert_Management.Models
{
    [Table("category")]
    public class Category
    {
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public ICollection<IncidentCategory> IncidentCategories { get; set; } = [];
    }
}
