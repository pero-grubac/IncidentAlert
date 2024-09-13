using IncidentAlert.Models.Dto;

namespace IncidentAlert.Services
{
    public interface IIncidentService
    {
        Task<IEnumerable<IncidentDto>> GetApproved();
        Task<IEnumerable<IncidentDto>> GetRequests();
        Task<IEnumerable<IncidentDto>> GetByCategoryId(int categoryId);
        Task<IncidentDto> GetById(int id);
        Task<IncidentDto> Add(IncidentDto incidentDto);
        Task<IncidentDto> Update(int id, IncidentDto incidentDto);
        Task Delete(int id);

    }
}
