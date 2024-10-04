using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Management.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IncidentController(IIncidentService incidentService) : ControllerBase
    {
        private readonly IIncidentService _service = incidentService;

        [Authorize(Roles = "MODERATOR")]
        [HttpGet("getAll")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ResponseIncidentDto>))]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetAll();

            return Ok(incidents);
        }

        [Authorize(Roles = "MODERATOR")]
        [HttpGet("getByCategoryId/{id:int}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ResponseIncidentDto>))]
        public async Task<IActionResult> GetByCategeoryId(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetByCategoryId(id);

            return Ok(incidents);
        }

        [Authorize(Roles = "MODERATOR")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(ResponseIncidentDto))]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetById(id);

            return Ok(incidents);
        }

        [Authorize(Roles = "MODERATOR")]
        [HttpGet("GetByCategoryName/{name}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ResponseIncidentDto>))]
        public async Task<IActionResult> GetAllByCategoryName(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetAllByCategoryName(name);

            return Ok(incidents);
        }

        [Authorize(Roles = "MODERATOR")]
        [HttpGet("GetAllOnDate/{date:datetime}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ResponseIncidentDto>))]
        public async Task<IActionResult> GetAllOnDate(DateTime date)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetAllOnDate(date);

            return Ok(incidents);
        }

        [Authorize(Roles = "MODERATOR")]
        [HttpGet("GetAllInDateRange")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ResponseIncidentDto>))]
        public async Task<IActionResult> GetAllInDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var incidents = await _service.GetAllInDateRange(startDate, endDate);

            return Ok(incidents);
        }

        [Authorize(Roles = "MODERATOR")]
        [HttpGet("GetAllByLocationName/{name}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ResponseIncidentDto>))]
        public async Task<IActionResult> GetAllByLocationName(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetAllByLocationName(name);

            return Ok(incidents);
        }
        //TREBABACE endpoint  izmjeniti tako da koristik moze da ga kreira
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Add([FromForm] IncidentDto incidentDto)
        {
            if (incidentDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.Add(incidentDto);

            return Ok();

        }

        [Authorize(Roles = "MODERATOR")]
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
