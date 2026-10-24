using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RegisterController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>Creates an account. It cannot sign in until an administrator approves it.</summary>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateUserDto newUser)
        {
            if (newUser == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            await _userService.Add(newUser);
            return Ok("Account created. An administrator has to approve it before you can sign in.");
        }
    }
}
