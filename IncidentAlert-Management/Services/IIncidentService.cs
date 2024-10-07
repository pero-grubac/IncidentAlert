using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Services
{
    public interface IIncidentService
    {
        Task<IEnumerable<ResponseIncidentDto>> GetAll();
        Task<IEnumerable<ResponseIncidentDto>> GetByCategoryId(int categoryId);
        Task<ResponseIncidentDto> GetById(int id);
        Task Add(IncidentDto incidentDto);
        Task<IncidentDto> Update(int id, IncidentDto incidentDto);
        Task Approve(int id, IncidentDto incidentDto);
        Task Delete(int id);
        Task<IEnumerable<ResponseIncidentDto>> GetAllByCategoryName(string categoryName);
        Task<IEnumerable<ResponseIncidentDto>> GetAllOnDate(DateTime date);
        Task<IEnumerable<ResponseIncidentDto>> GetAllInDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ResponseIncidentDto>> GetAllByLocationName(string locationName);
        Task<ResponseIncidentDto> ChnageStatus(int id, string status);


    }
}
