using IncidentAlert.Models.Dto;

namespace IncidentAlert.Services
{
    public interface IIncidentService
    {
        Task<IEnumerable<IncidentDto>> GetApproved();
        Task<IEnumerable<IncidentDto>> GetRequests();
        Task<IncidentDto> GetById(int id);
        Task<IncidentDto> Add(IncidentDto incidentDto);
        Task<IncidentDto> Update(int id, IncidentDto incident);
        Task Delete(int id);

    }
}
