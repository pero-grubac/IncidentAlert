using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
