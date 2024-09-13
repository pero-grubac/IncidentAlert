using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController(ILocationService locationService) : ControllerBase
    {
        private readonly ILocationService _service = locationService;

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LocationDto>))]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var locations = await _service.GetAll();

            return Ok(locations);
        }
    }
}
