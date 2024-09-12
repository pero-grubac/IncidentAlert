namespace IncidentAlert.Models.Dto
{
    public class IncidentDto : BaseDto<int>
    {
        public string Text { get; set; } = string.Empty;
        public Location Location { get; set; } = null!;
        public ICollection<CategoryDto> Categories { get; set; } = [];
    }
}
