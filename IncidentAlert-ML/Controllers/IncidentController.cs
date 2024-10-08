using IncidentAlert_ML.Model;
using IncidentAlert_ML.Service;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_ML.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IncidentController(IHttpClientFactory httpClientFactory, IIncidentGroupingService incidentGroupingService)
        : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IIncidentGroupingService _incidentGroupingService = incidentGroupingService;

        [HttpGet("grouped-incidents")]
        public async Task<IActionResult> GetGroupedIncidents()
        {
            var client = _httpClientFactory.CreateClient("IncidentAlert");
            var response = await client.GetAsync("Incident/getAllSimple");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to retrieve incidents.");
            }

            var incidents = await response.Content.ReadFromJsonAsync<List<SimpleIncident>>();
            if (incidents == null)
            {
                return BadRequest("No incidents were found.");
            }

            var groupedIncidents = await _incidentGroupingService.GroupIncidentsByText(incidents);

            return Ok(groupedIncidents);
        }
    }
}
