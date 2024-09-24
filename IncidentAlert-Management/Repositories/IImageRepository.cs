
using IncidentAlert_Management.Models;

namespace IncidentAlert_Management.Repositories
{
    public interface IImageRepository
    {
        Task<IEnumerable<Image>> GetByIncidentId(int incidentId);
        Task<Image?> GetById(int id);
        Task Add(Image image);
        Task Delete(int id);
    }
}
