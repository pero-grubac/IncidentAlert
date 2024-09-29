using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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


        [HttpPost("oauth")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<IActionResult> LoginOAuth([FromBody] OAuth data)
        {

            var username = Regex.Replace(data.Username!, @"[^a-zA-Z0-9]", "");

            if (string.IsNullOrWhiteSpace(data.Email) || string.IsNullOrWhiteSpace(data.GoogleId)
                        || string.IsNullOrWhiteSpace(username))
                return BadRequest();

            if (!data.Email.EndsWith("@student.etf.unibl.org"))
                return BadRequest("Email domain not allowed.");


            var oauth = new OAuth
            {
                GoogleId = data.GoogleId!,
                Email = data.Email!,
                Username = username!
            };
            var (result, token) = await _userService.OAuth(oauth);

            return result switch
            {
                OAuthResult.Created => CreatedAtAction(nameof(LoginOAuth), new { /* include relevant info */ }),
                OAuthResult.Failed => BadRequest("OAuth failed. Please try again."),
                OAuthResult.LoggedIn => Ok(token), // Return JWT for existing user
                _ => BadRequest("Unknown error occurred.")
            };
        }
    }
}
