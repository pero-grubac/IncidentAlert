using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Management.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "MODERATOR")]

    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _service = categoryService;

        [Authorize(Roles = "MODERATOR")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categories = await _service.GetAll();

            return Ok(categories);
        }

        [Authorize(Roles = "MODERATOR")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _service.GetById(id);
            return Ok(category);

        }

        [Authorize(Roles = "MODERATOR")]
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Add([FromBody] CategoryDto newCategory)
        {
            if (newCategory == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _service.Add(newCategory);

            return Ok(category);

        }

        [Authorize(Roles = "MODERATOR")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto newCategory)
        {
            if (newCategory == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _service.Update(id, newCategory);

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
