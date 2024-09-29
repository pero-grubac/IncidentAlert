using AutoMapper;
using IncidentAlert_Management.Exceptions;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Repositories;
using Microsoft.AspNetCore.Identity;

namespace IncidentAlert_Management.Services.Implementation
{
    public class UserService(IMapper mapper, UserManager<ApplicationUser> userManager,
                            IJwtService jwtService, IUserRepository userRepository,
                            ICustomPasswordService customPasswordService) : IUserService
    {
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICustomPasswordService _customPasswordService = customPasswordService;
        public async Task Add(CreateUserDto user)
        {
            var userdto = await _userManager.FindByNameAsync(user.Username);
            if (userdto != null)
                throw new EntityCanNotBeCreatedException($"User with username {user.Username} already exists.");

            var newUser = new ApplicationUser
            {
                UserName = user.Username,
                Email = user.Email,
                Role = RoleEnum.NOTITLE,
            };
            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                throw new EntityCanNotBeCreatedException($"Failed to create user: {user.Username}");
            }
        }

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            return _mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserDto>>(await _userRepository.GetAll());
        }

        public async Task<string> Login(LoginDto loginUser)
        {
            var user = await _userManager.FindByNameAsync(loginUser.Username);
            if (user == null)
                throw new EntityCanNotBeCreatedException("Invalid username or password.");

            if (user.Role == RoleEnum.NOTITLE)
                throw new EntityCanNotBeCreatedException("Invalid username or password.");

            var result = await _userManager.CheckPasswordAsync(user, loginUser.Password);
            if (!result)
                throw new EntityCanNotBeCreatedException("Invalid username or password.");
            return _jwtService.GenerateJwtToken(user);
        }

        public async Task<(OAuthResult result, string token)> OAuth(OAuth oauth)
        {
            var existingUserByGoogleId = await _userRepository.GetByGoogleId(oauth.GoogleId);
            if (existingUserByGoogleId != null)
            {
                return HandleExistingUserLogin(existingUserByGoogleId);
            }

            var existingUserByEmail = await _userRepository.GetByEmail(oauth.Email);
            if (existingUserByEmail != null)
            {
                return await HandleExistingUserWithEmail(existingUserByEmail, oauth);
            }

            var username = await GenerateUniqueUsername(oauth.Username);
            var newUser = new ApplicationUser
            {
                Email = oauth.Email,
                UserName = username,
                Role = RoleEnum.NOTITLE,
                GoogleId = oauth.GoogleId,
            };
            var password = _customPasswordService.GenerateRandomPassword();
            var result = await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
            {
                throw new EntityCanNotBeCreatedException($"Failed to create user: {username}");
            }

            return (OAuthResult.Created, string.Empty);
        }

        private async Task<string> GenerateUniqueUsername(string baseUsername)
        {
            var userByUsername = await _userRepository.GetByUsername(baseUsername);
            if (userByUsername == null)
            {
                return baseUsername;
            }

            Random random = new();
            string newUsername;
            do
            {
                newUsername = baseUsername + random.Next(1, 10000); // Dodaj random broj da bi username bio jedinstven
            } while (await _userRepository.GetByUsername(newUsername) != null);

            return newUsername;
        }

        private async Task<(OAuthResult result, string token)> HandleExistingUserWithEmail(ApplicationUser user, OAuth oauth)
        {
            user.GoogleId = oauth.GoogleId;
            await _userRepository.Update(user);



            var isLoginSuccessful = await _userRepository.Exists(u => u.UserName == user.UserName!
                    && u.PasswordHash == user.PasswordHash!);
            if (isLoginSuccessful)
            {
                var token = _jwtService.GenerateJwtToken(user);
                return (OAuthResult.LoggedIn, token);
            }

            return (OAuthResult.Failed, string.Empty);
        }



        private (OAuthResult result, string token) HandleExistingUserLogin(ApplicationUser user)
        {
            if (user.Role == RoleEnum.NOTITLE)
                throw new EntityDoesNotExistException("Invalid username or password.");

            var token = _jwtService.GenerateJwtToken(user);
            return (OAuthResult.LoggedIn, token);
        }
    }
}
