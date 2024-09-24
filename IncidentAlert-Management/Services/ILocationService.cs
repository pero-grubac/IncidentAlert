using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAll();
        Task<LocationDto?> GetById(int id);
        Task<LocationDto> Add(LocationDto locationDto);
        Task<LocationDto> Update(int id, LocationDto locationDto);
        Task Delete(int id);
    }
}
