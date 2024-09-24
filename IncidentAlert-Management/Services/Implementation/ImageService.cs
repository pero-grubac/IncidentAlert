using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Repositories;

namespace IncidentAlert.Services.Implementation
{
    public class ImageService(IWebHostEnvironment environment, IImageRepository imageRepository,
        IIncidentRepository incidentRepository, IHttpContextAccessor httpContextAccessor) : IImageService
    {
        private readonly IWebHostEnvironment _environment = environment;
        private readonly IImageRepository _imageRepository = imageRepository;
        private readonly IIncidentRepository _incidentRepository = incidentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

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
            var filePath = Path.Combine(uploadsFolderPath, file.FileName);
            try
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
            }
            catch (Exception ex)
            {
                throw new FileSaveException("An error occurred while saving the file", ex);
            }
            var image = new Image
            {
                FilePath = $"/uploads/{incidentId}/{file.FileName}",
                IncidentId = incidentId
            };
            await _imageRepository.Add(image);
        }

        public async Task Delete(int imageId)
        {
            var image = await _imageRepository.GetById(imageId)
                ?? throw new EntityDoesNotExistException("Image not found");

            var filePath = Path.Combine(_environment.WebRootPath, image.FilePath.TrimStart('/'));
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new EntityCannotBeDeletedException($"An error occurred while deleting the file: {image.FilePath}", ex);
            }

            await _imageRepository.Delete(imageId);
        }

        public async Task<IEnumerable<Image>> GetAllByIncidentId(int incidentId)
        {
            return await _imageRepository.GetByIncidentId(incidentId);

        }

        public async Task<ICollection<string>?> GetImageNames(int incidentId)
        {
            var images = await _imageRepository.GetByIncidentId(incidentId);

            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            var imageUrls = images.Select(image =>
            {
                // Generišemo relativni URL put do slike
                var relativePath = image.FilePath.TrimStart('/');

                // Kombinujemo sa osnovnim URL-om aplikacije
                var url = $"{request.Scheme}://{request.Host}/{relativePath}";

                // Enkodujemo URL da bi specijalni karakteri bili validni
                return Uri.EscapeUriString(url);  // EscapeUriString enkoduje specijalne karaktere
            }).ToList();

            return imageUrls;
        }
    }
}
