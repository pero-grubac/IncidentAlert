using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Services
{
    public interface IUserService
    {
        /// <summary>Creates an account that waits for an administrator's approval.</summary>
        Task Add(CreateUserDto user);

        /// <summary>Returns a JWT for an approved account.</summary>
        Task<string> Login(LoginDto loginUser);

        /// <summary>Signs in with a Google ID token, creating a pending account the first time.</summary>
        Task<(OAuthResult result, string token)> LoginWithGoogle(string idToken);

        Task<IEnumerable<UserDto>> GetAll();

        /// <summary>Gives a pending account the moderator role.</summary>
        Task<UserDto> Approve(string id, string actingUsername);

        /// <summary>Takes the moderator role away again.</summary>
        Task<UserDto> Revoke(string id, string actingUsername);

        Task Delete(string id, string actingUsername);
    }
}
