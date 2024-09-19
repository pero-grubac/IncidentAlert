using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Repositories;

namespace IncidentAlert.Services.Implementation
{
    public class ImageService(IWebHostEnvironment environment, IImageRepository imageRepository,
        IIncidentRepository incidentRepository) : IImageService
    {
        private readonly IWebHostEnvironment _environment = environment;
        private readonly IImageRepository _imageRepository = imageRepository;
        private readonly IIncidentRepository _incidentRepository = incidentRepository;
        public async Task Add(IFormFile file, int incidentId)
        {

            if (file == null || file.Length == 0)
                throw new FileEmptyException("File is empty");

            Incident incident = await _incidentRepository.GetById(incidentId)
                ?? throw new EntityDoesNotExistException("Article not found.");

            var uploadsFolderPath = Path.Combine(_environment.WebRootPath, "uploads", incident.Id.ToString());
            if (!Directory.Exists(uploadsFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }
                catch (Exception ex)
                {
                    throw new DirectoryCreationException("Could not create directory for uploads", ex);
                }
            }
        }

        public Task Delete(int incidentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IFormFile>> GetAllByIncidentId()
        {
            throw new NotImplementedException();
        }
    }
}
