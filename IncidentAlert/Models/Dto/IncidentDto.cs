namespace IncidentAlert.Models.Dto
{
    public class IncidentDto
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }

        public LocationDto Location { get; set; } = null!;
        public ICollection<CategoryDto> Categories { get; set; } = [];
    }
}
