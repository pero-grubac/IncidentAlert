using IncidentAlert_Statistics.Models;
using IncidentAlert_Statistics.Service;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Statistics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController(IHttpClientFactory httpClientFactory, IStatisticsService service)
        : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IStatisticsService _service = service;

        [HttpGet("LocationWithMostIncidents")]
        public async Task<IActionResult> GetLocationWithMostIncidents()
        {
            var client = _httpClientFactory.CreateClient("IncidentAlert");
            var response = await client.GetAsync("Incident/getAll");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to retrieve incidents.");
            }

            var incidents = await response.Content.ReadFromJsonAsync<List<IncidentDto>>();
            if (incidents == null)
            {
                return BadRequest("No incidents were found.");
            }

            var newResult = _service.GetLocationWithMostIncidents(incidents);

            return Ok(newResult);
        }

        [HttpGet("LocationWithMostIncidentsPerCategory")]
        public async Task<IActionResult> GetLocationWithMostIncidentsPerCategory()
        {
            var client = _httpClientFactory.CreateClient("IncidentAlert");
            var response = await client.GetAsync("Incident/getAll");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to retrieve incidents.");
            }

            var incidents = await response.Content.ReadFromJsonAsync<List<IncidentDto>>();
            if (incidents == null)
            {
                return BadRequest("No incidents were found.");
            }

            var newResult = _service.GetLocationWithMostIncidentsPerCategory(incidents);

            return Ok(newResult);
        }

        [HttpGet("NumberOfIncidentsPerCategory")]
        public async Task<IActionResult> GetNumberOfIncidentsPerCategory()
        {
            var client = _httpClientFactory.CreateClient("IncidentAlert");
            var response = await client.GetAsync("Incident/getAll");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to retrieve incidents.");
            }

            var incidents = await response.Content.ReadFromJsonAsync<List<IncidentDto>>();
            if (incidents == null)
            {
                return BadRequest("No incidents were found.");
            }

            var newResult = _service.GetNumberOfIncidentsPerCategory(incidents);

            return Ok(newResult);
        }
    }
}
