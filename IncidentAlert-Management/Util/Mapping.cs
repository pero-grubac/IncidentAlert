using Contracts.Category;
using Contracts.Incident;
using Contracts.Location;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Util
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

        public static CategoryCreatedEvent ToCreatedEvent(this Category category) => new() { Id = category.Id, Name = category.Name };

        public static CategoryUpdateEvent ToUpdateEvent(this Category category) => new() { Id = category.Id, Name = category.Name };

        public static CategoryDeleteEvent ToDeleteEvent(this Category category) => new() { Id = category.Id };

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

        public static LocationCreateEvent ToMessage(this Location location) => new()
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
        };

        // ---------- Images ----------

        public static ImageData ToDto(this Contracts.Image.ImageData message) => new() { FileName = message.FileName, Content = message.Content };

        public static Contracts.Image.ImageData ToMessage(this ImageData dto) => new() { FileName = dto.FileName, Content = dto.Content };

        // ---------- Incidents ----------

        /// <summary>A citizen's report arriving from the IncidentAlert service.</summary>
        public static IncidentDto ToDto(this IncidentCreateEvent message) => new()
        {
            Id = message.Id,
            Title = message.Title,
            Text = message.Text,
            DateTime = message.DateTime,
            Location = message.Location?.ToDto()!,
            Categories = message.Categories.ToList(),
            ImagesData = message.ImagesData.Select(i => i.ToDto()).ToList(),
        };

        public static IncidentDto ToDto(this Incident incident) => new()
        {
            Id = incident.Id,
            Title = incident.Title,
            Text = incident.Text,
            DateTime = incident.DateTime,
            Status = incident.Status,
            Location = incident.Location?.ToDto()!,
            Categories = incident.IncidentCategories.Select(ic => ic.Category.Name).ToList(),
        };

        /// <summary>Image URLs are added by the service, which knows the request's host.</summary>
        public static ResponseIncidentDto ToResponse(this Incident incident) => new()
        {
            Id = incident.Id,
            Title = incident.Title,
            Text = incident.Text,
            DateTime = incident.DateTime,
            Status = incident.Status,
            Location = incident.Location?.ToDto()!,
            Categories = incident.IncidentCategories.Select(ic => ic.Category.ToDto()).ToList(),
        };

        /// <summary>The message that publishes an approved incident. Photos are attached by the caller.</summary>
        public static IncidentApprovedEvent ToApprovedEvent(this Incident incident) => new()
        {
            Id = incident.Id,
            Title = incident.Title,
            Text = incident.Text,
            DateTime = incident.DateTime,
            Location = incident.Location?.ToMessage(),
            Categories = incident.IncidentCategories.Select(ic => ic.Category.Name).ToList(),
        };

        // ---------- Users ----------

        public static UserDto ToDto(this ApplicationUser user) => new()
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Role = user.Role,
            SignsInWithGoogle = !string.IsNullOrEmpty(user.GoogleId),
        };
    }
}
