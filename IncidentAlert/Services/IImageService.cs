using IncidentAlert.Models;

namespace IncidentAlert.Services
{
    public interface IImageService
    {
        Task Add(IFormFile file, int incidentId);
        Task Delete(int incidentId);
        Task<IEnumerable<Image>> GetAllByIncidentId(int incidentId);
    }
}
