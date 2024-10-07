namespace Contracts.Location
{
    public record LocationCreateEvent
    {
        public int Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
