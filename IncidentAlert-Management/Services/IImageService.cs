using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Services
{
    public interface IImageService
    {
        Task Add(ImageData file, int incidentId);
        Task Delete(int incidentId);
        Task<IEnumerable<Image>> GetAllByIncidentId(int incidentId);
        Task<ICollection<string>?> GetImageNames(int incidentId);
        Task<ICollection<IFormFile>> GetImagesAsFiles(int incidentId);
    }
}
