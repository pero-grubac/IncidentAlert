using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Management.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "MODERATOR")]
    public class LocationController(ILocationService locationService) : ControllerBase
    {
        private readonly ILocationService _service = locationService;
        [Authorize(Roles = "MODERATOR")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LocationDto>))]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var locations = await _service.GetAll();

            return Ok(locations);
        }
        [Authorize(Roles = "MODERATOR")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(LocationDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var location = await _service.GetById(id);
            return Ok(location);

        }

        [Authorize(Roles = "MODERATOR")]
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Add([FromBody] LocationDto newLocation)
        {
            if (newLocation == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var location = await _service.Add(newLocation);

            return Ok(location);
        }

        [Authorize(Roles = "MODERATOR")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] LocationDto newLocation)
        {
            if (newLocation == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var location = await _service.Update(id, newLocation);

            return Ok(location);
        }
        [Authorize(Roles = "MODERATOR")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.Delete(id);

            return Ok("Succesfully deleted");
        }
    }
}
