namespace IncidentAlert_Statistics.Models
{
    public record LocationIncidentCount
    {
        public int IncidentCount { get; set; }
        public string LocationName { get; set; }
    }
}
