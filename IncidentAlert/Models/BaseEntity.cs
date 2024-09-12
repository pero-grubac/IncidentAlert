using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentAlert.Models
{
    public abstract class BaseEntity<T>
    {
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; } = default!;
    }
}
