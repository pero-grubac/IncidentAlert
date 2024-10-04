using IncidentAlert_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Management.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        [Authorize(Roles = "MODERATOR")]
        [HttpGet]
        public IActionResult GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var statusList = Enum.GetValues(typeof(StatusEnum))
                                 .Cast<StatusEnum>()
                                 .Where(s => s != StatusEnum.ACCEPTED) // Exclude the ACCEPTED status
                                 .Select(s => new { Id = (int)s, Name = s.ToString() })
                                 .ToList();
            return Ok(statusList);
        }
    }
}
