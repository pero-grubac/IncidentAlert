using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categories = await _categoryService.GetAllAsync();

            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.GetAsync(id);
            return Ok(category);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CategoryDto newCategory)
        {
            if (newCategory == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.AddAsync(newCategory);

            return Ok(category);

        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto newCategory)
        {
            if (newCategory == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != newCategory.Id)
                return BadRequest(ModelState);

            var category = await _categoryService.UpdateAsync(id, newCategory);

            return Ok(category);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _categoryService.DeleteAsync(id);

            return Ok("Succesfully deleted");
        }
    }
}
