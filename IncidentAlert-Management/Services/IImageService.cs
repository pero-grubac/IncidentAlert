using IncidentAlert_Management.Models;

namespace IncidentAlert_Management.Services
{
    public interface IImageService
    {
        Task Add(IFormFile file, int incidentId);
        Task Delete(int incidentId);
        Task<IEnumerable<Image>> GetAllByIncidentId(int incidentId);
        Task<ICollection<string>?> GetImageNames(int incidentId);
    }
}
