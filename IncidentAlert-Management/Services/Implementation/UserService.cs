using IncidentAlert_Management.Exceptions;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Repositories;
using IncidentAlert_Management.Util;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace IncidentAlert_Management.Services.Implementation
{
    public class UserService(UserManager<ApplicationUser> userManager, IJwtService jwtService,
                            IUserRepository userRepository, ICustomPasswordService customPasswordService,
                            IGoogleTokenValidator googleTokenValidator, GoogleSettings googleSettings) : IUserService
    {
        public const string PendingMessage = "Your account is waiting for an administrator's approval.";

        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICustomPasswordService _customPasswordService = customPasswordService;
        private readonly IGoogleTokenValidator _googleTokenValidator = googleTokenValidator;
        private readonly GoogleSettings _googleSettings = googleSettings;

        public async Task Add(CreateUserDto user)
        {
            if (await _userManager.FindByNameAsync(user.Username) != null)
                throw new EntityCanNotBeCreatedException($"User with username {user.Username} already exists.");

            // Self-registered accounts cannot moderate until an administrator approves them.
            var newUser = new ApplicationUser
            {
                UserName = user.Username,
                Email = user.Email,
                Role = RoleEnum.NOTITLE,
            };
            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded)
                throw new EntityCanNotBeCreatedException(
                    $"Failed to create user: {string.Join(" ", result.Errors.Select(e => e.Description))}");
        }

        public async Task<string> Login(LoginDto loginUser)
        {
            var user = await _userManager.FindByNameAsync(loginUser.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginUser.Password))
                throw new EntityCanNotBeCreatedException("Invalid username or password.");

            // Only said after the password matched, so it does not reveal which usernames exist.
            if (user.Role == RoleEnum.NOTITLE)
                throw new AccountPendingException(PendingMessage);

            return _jwtService.GenerateJwtToken(user);
        }

        public async Task<(OAuthResult result, string token)> LoginWithGoogle(string idToken)
        {
            // The token is verified with Google; nothing the browser claims about the user is trusted.
            var identity = await _googleTokenValidator.Validate(idToken)
                ?? throw new UnauthorizedAccessException("Google sign-in failed.");

            if (!identity.EmailVerified ||
                !identity.Email.EndsWith("@" + _googleSettings.AllowedDomain, StringComparison.OrdinalIgnoreCase))
                throw new EntityCanNotBeCreatedException($"Only @{_googleSettings.AllowedDomain} accounts can sign in with Google.");

            var user = await _userRepository.GetByGoogleId(identity.Subject);
            if (user == null)
            {
                user = await _userRepository.GetByEmail(identity.Email);
                if (user != null)
                {
                    // Google has verified that this person owns the address, so the accounts can be linked.
                    user.GoogleId = identity.Subject;
                    await _userRepository.Update(user);
                }
            }

            if (user == null)
            {
                var username = await GenerateUniqueUsername(identity.Name, identity.Email);
                var newUser = new ApplicationUser
                {
                    Email = identity.Email,
                    EmailConfirmed = true,
                    UserName = username,
                    Role = RoleEnum.NOTITLE,
                    GoogleId = identity.Subject,
                };
                var created = await _userManager.CreateAsync(newUser, _customPasswordService.GenerateRandomPassword(16));
                if (!created.Succeeded)
                    throw new EntityCanNotBeCreatedException($"Failed to create user: {username}");
                return (OAuthResult.Created, string.Empty);
            }

            return user.Role == RoleEnum.NOTITLE
                ? (OAuthResult.Pending, string.Empty)
                : (OAuthResult.LoggedIn, _jwtService.GenerateJwtToken(user));
        }

        public async Task<IEnumerable<UserDto>> GetAll() =>
            (await _userRepository.GetAll())
                .OrderBy(u => u.Role == RoleEnum.NOTITLE ? 0 : 1)
                .ThenBy(u => u.UserName)
                .Select(u => u.ToDto())
                .ToList();

        public Task<UserDto> Approve(string id, string actingUsername) => ChangeRole(id, actingUsername, RoleEnum.MODERATOR);

        public Task<UserDto> Revoke(string id, string actingUsername) => ChangeRole(id, actingUsername, RoleEnum.NOTITLE);

        public async Task Delete(string id, string actingUsername)
        {
            var user = await EditableUser(id, actingUsername);
            await _userManager.DeleteAsync(user);
        }

        private async Task<UserDto> ChangeRole(string id, string actingUsername, RoleEnum role)
        {
            var user = await EditableUser(id, actingUsername);
            user.Role = role;
            await _userRepository.Update(user);
            return user.ToDto();
        }

        /// <summary>Administrators manage other people's accounts, never their own or another administrator's.</summary>
        private async Task<ApplicationUser> EditableUser(string id, string actingUsername)
        {
            var user = await _userRepository.GetById(id)
                ?? throw new EntityDoesNotExistException($"User {id} does not exist.");
            if (user.UserName == actingUsername || user.Role == RoleEnum.ADMIN)
                throw new ArgumentException("Administrator accounts cannot be changed here.");
            return user;
        }

        private async Task<string> GenerateUniqueUsername(string name, string email)
        {
            var baseUsername = Regex.Replace(name, "[^a-zA-Z0-9]", "");
            if (string.IsNullOrEmpty(baseUsername))
                baseUsername = Regex.Replace(email.Split('@')[0], "[^a-zA-Z0-9]", "");
            if (string.IsNullOrEmpty(baseUsername))
                baseUsername = "user";

            var username = baseUsername;
            for (var i = 2; await _userRepository.GetByUsername(username) != null; i++)
                username = $"{baseUsername}{i}";
            return username;
        }
    }
}
