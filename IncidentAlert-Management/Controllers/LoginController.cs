using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IncidentAlert_Management.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost]
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
        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action(nameof(LoginOAuth), "Login");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpPost("oauth")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<IActionResult> LoginOAuth()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest(); // Autentifikacija nije uspela

            // Dobijanje informacija o korisniku
            var claims = authenticateResult.Principal.Identities
                .FirstOrDefault()?.Claims;

            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var googleId = claims?.FirstOrDefault(c => c.Type == "sub")?.Value;
            var fullName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (email == string.Empty || googleId == string.Empty || fullName == string.Empty)
                return BadRequest();

            var oauth = new OAuth
            {
                GoogleId = googleId!,
                Email = email!,
                Username = fullName!
            };
            var (result, token) = await _userService.OAuth(oauth);

            if (result == OAuthResult.Created)
            {
                return Created(); // Kreiran novi korisnik
            }
            if (result == OAuthResult.LoggedIn)
            {
                return Ok(token); // Vraća JWT za postojeći korisnik
            }
        }
    }
}
