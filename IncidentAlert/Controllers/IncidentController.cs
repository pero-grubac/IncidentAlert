using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentController(IIncidentService incidentService) : ControllerBase
    {
        private readonly IIncidentService _service = incidentService;

        [HttpGet("getApproved")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IncidentDto>))]
        public async Task<IActionResult> GetApproved()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetApproved();

            return Ok(incidents);
        }

        [HttpGet("getRequests")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IncidentDto>))]
        public async Task<IActionResult> GetRequested()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetRequests();

            return Ok(incidents);
        }
        [HttpGet("getByCategoryId/{id:int}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IncidentDto>))]
        public async Task<IActionResult> GetByCategeoryId(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetByCategoryId(id);

            return Ok(incidents);
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(IncidentDto))]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetById(id);

            return Ok(incidents);
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Add([FromBody] IncidentDto incidentDto)
        {
            if (incidentDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _service.Add(incidentDto);

            return Ok(category);

        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] IncidentDto incidentDto)
        {
            if (incidentDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _service.Update(id, incidentDto);

            return Ok(category);
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
