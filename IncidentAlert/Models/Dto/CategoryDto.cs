namespace IncidentAlert.Models.Dto
{
    public class CategoryDto : BaseDto<int>
    {
        public string Name { get; set; } = string.Empty;
    }
}
