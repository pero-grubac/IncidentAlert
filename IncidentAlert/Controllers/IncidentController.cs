﻿using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IncidentController(IIncidentService incidentService) : ControllerBase
    {
        private readonly IIncidentService _service = incidentService;

        [HttpGet("getAll")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IncidentDto>))]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetAll();

            return Ok(incidents);
        }

        [HttpGet("getAllSimple")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IncidentDto>))]
        public async Task<IActionResult> GetAllSimple()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetAllSimple();

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


        [HttpGet("GetByCategoryName/{name}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IncidentDto>))]
        public async Task<IActionResult> GetAllByCategoryName(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetAllByCategoryName(name);

            return Ok(incidents);
        }

        [HttpGet("GetAllOnDate/{date:datetime}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IncidentDto>))]
        public async Task<IActionResult> GetAllOnDate(DateTime date)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetAllOnDate(date);

            return Ok(incidents);
        }

        [HttpGet("GetAllInDateRange")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IncidentDto>))]
        public async Task<IActionResult> GetAllInDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var incidents = await _service.GetAllInDateRange(startDate, endDate);

            return Ok(incidents);
        }

        [HttpGet("GetAllByLocationName/{name}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<IncidentDto>))]
        public async Task<IActionResult> GetAllByLocationName(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var incidents = await _service.GetAllByLocationName(name);

            return Ok(incidents);
        }

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
