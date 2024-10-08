namespace IncidentAlert_Statistics.Models
{
    public record LocationCategoryIncidentCount
    {
        public string LocationName { get; set; }
        public string CategoryName { get; set; }
        public int IncidentCount { get; set; }
    }
}
