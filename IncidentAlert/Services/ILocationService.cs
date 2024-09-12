using IncidentAlert.Models.Dto;

namespace IncidentAlert.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAll();
        Task<LocationDto?> GetById(int id);
        Task<LocationDto> Add(LocationDto entity);
        Task<LocationDto> Update(int id, LocationDto entity);
        Task Delete(int id);
    }
}
