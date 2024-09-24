using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Services
{
    public interface IUserService
    {
        Task<ICollection<UserDto>> GetAll();
        Task Add(UserDto user);
        Task<string> Login(UserDto loginUser);
        Task Logout(UserDto logoutUser);
    }
}
