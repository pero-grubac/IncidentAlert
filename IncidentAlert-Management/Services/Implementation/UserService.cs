using AutoMapper;
using IncidentAlert_Management.Exceptions;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Repositories;
using Microsoft.AspNetCore.Identity;

namespace IncidentAlert_Management.Services.Implementation
{
    public class UserService(IMapper mapper, UserManager<ApplicationUser> userManager,
                            IJwtService jwtService, IUserRepository userRepository) : IUserService
    {
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUserRepository _userRepository = userRepository;
        public async Task Add(CreateUserDto user)
        {
            var userdto = await _userManager.FindByNameAsync(user.Username);
            if (userdto != null)
                throw new EntityCanNotBeCreatedException($"User with username {user.Username} already exists.");

            var newUser = new ApplicationUser
            {
                UserName = user.Username,
                Email = user.Email,
                Role = RoleEnum.MODERATOR,
            };
            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new EntityCanNotBeCreatedException($"Failed to create user: {errors}");
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

            var result = await _userManager.CheckPasswordAsync(user, loginUser.Password);
            if (!result)
                throw new EntityCanNotBeCreatedException("Invalid username or password.");
            return _jwtService.GenerateJwtToken(user);
        }


    }
}
