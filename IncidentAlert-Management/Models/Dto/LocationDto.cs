namespace IncidentAlert_Management.Models.Dto
{
    public class LocationDto
    {
        public int Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; } = string.Empty;

    }
}
