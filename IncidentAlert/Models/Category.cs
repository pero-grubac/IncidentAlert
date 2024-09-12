using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace IncidentAlert.Models
{
    [Table("category")]
    public class Category : BaseEntity<int>
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public ICollection<IncidentCategory> IncidentCategories { get; set; } = [];
    }
}
