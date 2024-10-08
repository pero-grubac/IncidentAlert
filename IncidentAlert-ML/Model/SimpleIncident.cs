namespace IncidentAlert_ML.Model
{
    public record SimpleIncident
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string Title { get; set; } = string.Empty;
        public SimpleLocation? Location { get; set; } = default;
    }
}
