using Contracts.Category;
using Contracts.Incident;
using Contracts.Location;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;

namespace IncidentAlert.Util
{
    /// <summary>
    /// Explicit mappings between entities, DTOs and message contracts.
    /// Written by hand so every copied field is visible and checked by the compiler.
    /// </summary>
    public static class Mapping
    {
        // ---------- Categories ----------

        public static CategoryDto ToDto(this Category category) => new() { Id = category.Id, Name = category.Name };

        public static Category ToEntity(this CategoryDto dto) => new() { Id = dto.Id, Name = dto.Name };

        // Ids are kept, so a category has the same id in the Management and the public database.
        public static CategoryDto ToDto(this CategoryCreatedEvent message) => new() { Id = message.Id, Name = message.Name };

        public static CategoryDto ToDto(this CategoryUpdateEvent message) => new() { Id = message.Id, Name = message.Name };

        // ---------- Locations ----------

        public static LocationDto ToDto(this Location location) => new()
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
        };

        public static Location ToEntity(this LocationDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
        };

        public static LocationDto ToDto(this LocationCreateEvent message) => new()
        {
            Id = message.Id,
            Name = message.Name,
            Latitude = message.Latitude,
            Longitude = message.Longitude,
        };

        public static LocationCreateEvent ToMessage(this LocationDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
        };

        // ---------- Incidents ----------

        public static IncidentDto ToDto(this Incident incident) => new()
        {
            Id = incident.Id,
            Title = incident.Title,
            Text = incident.Text,
            DateTime = incident.DateTime,
            Location = incident.Location?.ToDto(),
            Categories = incident.IncidentCategories.Select(ic => ic.Category.Name).ToList(),
        };

        /// <summary>Image URLs are added by the service, which knows the request's host.</summary>
        public static ResponseIncidentDto ToResponse(this Incident incident) => new()
        {
            Id = incident.Id,
            Title = incident.Title,
            Text = incident.Text,
            DateTime = incident.DateTime,
            Location = incident.Location?.ToDto()!,
            Categories = incident.IncidentCategories.Select(ic => ic.Category.ToDto()).ToList(),
        };

        public static SimpleIncident ToSimple(this Incident incident) => new()
        {
            Id = incident.Id,
            Title = incident.Title,
            Text = incident.Text,
            DateTime = incident.DateTime,
            Location = incident.Location is null ? null : new SimpleLocation { Id = incident.Location.Id, Name = incident.Location.Name },
        };

        /// <summary>A citizen's report, on its way to the moderation queue. Photos are attached by the caller.</summary>
        public static IncidentCreateEvent ToCreateEvent(this IncidentDto dto) => new()
        {
            Id = dto.Id,
            Title = dto.Title,
            Text = dto.Text,
            DateTime = dto.DateTime,
            Location = dto.Location?.ToMessage(),
            Categories = dto.Categories.ToList(),
        };

        public static ApprovedIncident ToApprovedIncident(this IncidentApprovedEvent message) => new()
        {
            Id = message.Id,
            Title = message.Title,
            Text = message.Text,
            DateTime = message.DateTime,
            Location = message.Location?.ToDto(),
            Categories = message.Categories.ToList(),
            ImagesData = message.ImagesData.Select(i => new ImageData { FileName = i.FileName, Content = i.Content }).ToList(),
        };
    }
}
