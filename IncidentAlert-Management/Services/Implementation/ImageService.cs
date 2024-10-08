using IncidentAlert_Management.Exceptions;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Repositories;

namespace IncidentAlert_Management.Services.Implementation
{
    public class ImageService(IWebHostEnvironment environment, IImageRepository imageRepository,
        IIncidentRepository incidentRepository, IHttpContextAccessor httpContextAccessor) : IImageService
    {
        private readonly IWebHostEnvironment _environment = environment;
        private readonly IImageRepository _imageRepository = imageRepository;
        private readonly IIncidentRepository _incidentRepository = incidentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task Add(ImageData file, int incidentId)
        {

            if (file.Content == null || file.FileName == null)
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
                await File.WriteAllBytesAsync(filePath, file.Content);
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

        public async Task<ICollection<IFormFile>> GetImagesAsFiles(int incidentId)
        {
            var uploadsFolderPath = Path.Combine(_environment.WebRootPath, "uploads", incidentId.ToString());
            var formFiles = new List<IFormFile>();

            if (Directory.Exists(uploadsFolderPath))
            {
                var imageFiles = Directory.GetFiles(uploadsFolderPath);
                foreach (var imagePath in imageFiles)
                {
                    var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    var formFile = new FormFile(fileStream, 0, fileStream.Length, "image", Path.GetFileName(imagePath))
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpeg" // Prilagodi prema tipu sadržaja
                    };
                    formFiles.Add(formFile);
                }
            }

            return formFiles;
        }
    }
}
