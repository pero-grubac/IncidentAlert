using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAll();
        Task Add(UserDto user);
        Task<string> Login(UserDto loginUser);
    }
}
