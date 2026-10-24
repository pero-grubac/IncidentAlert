using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IncidentAlert_Management.Controllers
{
    /// <summary>Account administration: approving new moderators.</summary>
    [Route("users")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        // The JWT's "sub" claim holds the username.
        private string CurrentUsername =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? string.Empty;

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
        public async Task<IActionResult> GetAll() => Ok(await _userService.GetAll());

        [HttpPut("{id}/approve")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Approve(string id) => Ok(await _userService.Approve(id, CurrentUsername));

        [HttpPut("{id}/revoke")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Revoke(string id) => Ok(await _userService.Revoke(id, CurrentUsername));

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.Delete(id, CurrentUsername);
            return NoContent();
        }
    }
}
