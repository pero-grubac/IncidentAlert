using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Management.Controllers
{
    [Route("register")]
    [ApiController]
    public class RegisterController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateUserDto newUser)
        {
            if (newUser == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            await _userService.Add(newUser);
            return Ok("Succesfully created");
        }
    }
}
