namespace IncidentAlert.Models.Dto
{
    public class LocationDto : BaseDto<int>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; } = string.Empty;

    }
}
