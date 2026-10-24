using IncidentAlert_Statistics.Models;
using IncidentAlert_Statistics.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Statistics.Controllers
{
    [ApiController]
    [Authorize(Roles = "MODERATOR,ADMIN")]
    [Route("[controller]")]
    public class StatisticsController(IHttpClientFactory httpClientFactory, IStatisticsService service)
        : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IStatisticsService _service = service;

        [HttpGet("LocationWithMostIncidents")]
        public Task<IActionResult> GetLocationWithMostIncidents() =>
            WithIncidents(incidents => _service.GetLocationWithMostIncidents(incidents));

        [HttpGet("LocationWithMostIncidentsPerCategory")]
        public Task<IActionResult> GetLocationWithMostIncidentsPerCategory() =>
            WithIncidents(incidents => _service.GetLocationWithMostIncidentsPerCategory(incidents));

        [HttpGet("NumberOfIncidentsPerCategory")]
        public Task<IActionResult> GetNumberOfIncidentsPerCategory() =>
            WithIncidents(incidents => _service.GetNumberOfIncidentsPerCategory(incidents));

        /// <summary>Every place with its coordinates and incident count, for a hot-spot map. Optionally only the last <paramref name="days"/> days.</summary>
        [HttpGet("LocationHeat")]
        [ProducesResponseType(200, Type = typeof(List<LocationHeat>))]
        public Task<IActionResult> GetLocationHeat([FromQuery] int? days = null) =>
            WithIncidents(incidents => _service.GetLocationHeat(incidents, days is > 0 ? DateTime.UtcNow.AddDays(-days.Value) : null));

        /// <summary>Incidents per day for the last <paramref name="days"/> days (1–365), oldest first, with empty days included.</summary>
        [HttpGet("IncidentsPerDay")]
        [ProducesResponseType(200, Type = typeof(List<DailyIncidentCount>))]
        public Task<IActionResult> GetIncidentsPerDay([FromQuery] int days = 30) =>
            WithIncidents(incidents => _service.GetIncidentsPerDay(incidents, Math.Clamp(days, 1, 365), DateOnly.FromDateTime(DateTime.UtcNow)));

        /// <summary>Loads the published incidents from the IncidentAlert API and passes them to <paramref name="compute"/>.</summary>
        private async Task<IActionResult> WithIncidents<T>(Func<List<IncidentDto>, T> compute)
        {
            var client = _httpClientFactory.CreateClient("IncidentAlert");
            var response = await client.GetAsync("Incident/getAll");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to retrieve incidents.");

            var incidents = await response.Content.ReadFromJsonAsync<List<IncidentDto>>();
            if (incidents == null)
                return BadRequest("No incidents were found.");

            return Ok(compute(incidents));
        }
    }
}
