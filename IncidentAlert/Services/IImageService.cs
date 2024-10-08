using IncidentAlert.Models;
using IncidentAlert.Models.Dto;

namespace IncidentAlert.Services
{
    public interface IImageService
    {
        Task Add(IFormFile file, int incidentId);
        Task Add(ImageData file, int incidentId);

        Task Delete(int incidentId);
        Task<IEnumerable<Image>> GetAllByIncidentId(int incidentId);
        Task<ICollection<string>?> GetImageNames(int incidentId);
    }
}
