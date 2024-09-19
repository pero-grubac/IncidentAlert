namespace IncidentAlert.Services
{
    public interface IImageService
    {
        Task Add(IFormFile file, int incidentId);
        Task Delete(int incidentId);
        Task<IEnumerable<IFormFile>> GetAllByIncidentId();
    }
}
