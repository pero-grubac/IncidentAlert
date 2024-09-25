using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Management.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        //  TODO login username/password
        // TODO create username/email/password
        [HttpPost("register")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
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
        public async Task<IActionResult> Login([FromBody] LoginDto user)
        {
            if (user == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            var jwt = await _userService.Login(user);
            return Ok(jwt);
        }
    }
}
