namespace IncidentAlert.Models.Dto
{
    public class ResponseIncidentDto
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public DateTime DateTime { get; set; }

        public LocationDto Location { get; set; } = null!;
        public ICollection<CategoryDto> Categories { get; set; } = [];
        public ICollection<string> Images { get; set; } = [];
    }
}
