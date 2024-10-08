namespace IncidentAlert_ML.Model
{
    public record IncidentGroup
    {
        public string GroupKey { get; set; } = string.Empty;
        public List<SimpleIncident> Incidents { get; set; } = new();

    }
}
