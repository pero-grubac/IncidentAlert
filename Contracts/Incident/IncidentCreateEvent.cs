using Contracts.Location;
using Microsoft.AspNetCore.Http;

namespace Contracts.Incident
{
    public record IncidentCreateEvent
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public DateTime DateTime { get; set; }
        public LocationCreateEvent? Location { get; set; } = default;
        public ICollection<string> Categories { get; set; } = [];

        public ICollection<IFormFile> Images { get; set; } = [];
    }
}
