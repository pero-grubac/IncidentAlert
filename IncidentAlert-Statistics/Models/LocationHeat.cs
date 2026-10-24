namespace IncidentAlert_Statistics.Models
{
    /// <summary>One place on the hot-spot map: where it is and how many incidents happened there.</summary>
    public record LocationHeat
    {
        public string LocationName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int IncidentCount { get; set; }
    }
}
