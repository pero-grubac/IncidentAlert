using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using IncidentAlert_Management.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAlert_Management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Login([FromBody] LoginDto user)
        {
            if (user == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(await _userService.Login(user));
        }

        /// <summary>Sign-in with a Google ID token. 201: account created and waiting for approval; 403: still waiting.</summary>
        [HttpPost("oauth")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> LoginOAuth([FromBody] GoogleLoginDto data)
        {
            if (string.IsNullOrWhiteSpace(data?.IdToken))
                return BadRequest("A Google ID token is required.");

            var (result, token) = await _userService.LoginWithGoogle(data.IdToken);

            return result switch
            {
                OAuthResult.LoggedIn => Ok(token),
                OAuthResult.Created => StatusCode(StatusCodes.Status201Created,
                    "Account created. An administrator has to approve it before you can sign in."),
                _ => StatusCode(StatusCodes.Status403Forbidden, UserService.PendingMessage),
            };
        }
    }
}
