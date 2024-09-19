using IncidentAlert.Models.Dto;

namespace IncidentAlert.Services
{
    public interface IIncidentService
    {
        Task<IEnumerable<IncidentDto>> GetAll();
        Task<IEnumerable<ResponseIncidentDto>> GetByCategoryId(int categoryId);
        Task<ResponseIncidentDto> GetById(int id);
        Task Add(IncidentDto incidentDto);
        Task<IncidentDto> Update(int id, IncidentDto incidentDto);
        Task Delete(int id);
        Task<IEnumerable<IncidentDto>> GetAllByCategoryName(string categoryName);
        Task<IEnumerable<IncidentDto>> GetAllOnDate(DateTime date);
        Task<IEnumerable<IncidentDto>> GetAllInDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<IncidentDto>> GetAllByLocationName(string locationName);

    }
}
