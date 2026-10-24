namespace IncidentAlert_Statistics.Models
{
    /// <summary>Incidents on one day, in total and per category (an incident counts once for each of its categories).</summary>
    public record DailyIncidentCount
    {
        public DateOnly Date { get; set; }
        public int Total { get; set; }
        public Dictionary<string, int> Categories { get; set; } = [];
    }
}
