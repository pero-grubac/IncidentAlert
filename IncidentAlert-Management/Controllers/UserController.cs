using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Management.Controllers
{
    [Route("users")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]

    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost("register")]
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

        [HttpPost("login")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto user)
        {
            if (user == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            var jwt = await _userService.Login(user);
            return Ok(jwt);
        }
    }
}
