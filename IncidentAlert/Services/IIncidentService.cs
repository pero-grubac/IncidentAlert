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
        Task<IEnumerable<IncidentDto>> GetAllByCategoryName(string categoryName);
        Task<IEnumerable<IncidentDto>> GetAllApprovedIncidentsOnDate(DateTime date);
        Task<IEnumerable<IncidentDto>> GetAllApprovedIncidentsInDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<IncidentDto>> GetAllByLocationName(string locationName);

    }
}
