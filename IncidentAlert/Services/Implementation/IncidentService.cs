using Contracts.Incident;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories;
using IncidentAlert.Util;
using MassTransit;

namespace IncidentAlert.Services.Implementation
{
    public class IncidentService(IIncidentRepository incidentRepository,
        ILocationRepository locationRepository, ICategoryRepository categoryRepository,
        IIncidentCategoryRepository incidentCategoryRepository, IImageService imageService,
        IPublishEndpoint publishEndpoint) : IIncidentService
    {
        private readonly IIncidentRepository _repository = incidentRepository;
        private readonly ILocationRepository _locationRepository = locationRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IIncidentCategoryRepository _incidentCategoryRepository = incidentCategoryRepository;
        private readonly IImageService _imageService = imageService;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        public async Task Add(IncidentDto incidentDto)
        {
            var incidentEvent = incidentDto.ToCreateEvent();
            incidentEvent.ImagesData = incidentDto.Images.Select(image => ConvertToImageData(image)).ToList();
            await _publishEndpoint.Publish(incidentEvent);
        }
        private Contracts.Image.ImageData ConvertToImageData(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            return new Contracts.Image.ImageData
            {
                FileName = file.FileName,
                Content = memoryStream.ToArray()
            };
        }
        public async Task Delete(int id)
        {
            var entity = await _repository.GetById(id) ?? throw new EntityDoesNotExistException($"Incident with ID {id} does not exist.");
            try
            {
                await _incidentCategoryRepository.DeleteAllWithIncidentId(id);
                var images = await _imageService.GetAllByIncidentId(id);
                foreach (var image in images.ToList())
                    await _imageService.Delete(image.Id);
                await _repository.Delete(entity);

            }
            catch (Exception ex)
            {
                throw new EntityCannotBeDeletedException($"Incident with ID {id} cannot be deleted. {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ResponseIncidentDto>> GetAllInDateRange(DateTime startDate, DateTime endDate)
        {
            var incidents = (await _repository.GetAllInDateRange(startDate.ToUniversalTime(), endDate.ToUniversalTime()))
                  .Select(i => i.ToResponse()).ToList();
            var response = await MapImageNames(incidents);
            return response;
        }


        public async Task<IEnumerable<ResponseIncidentDto>> GetAllOnDate(DateTime date)
        {
            var incidents = (await _repository.GetAllOnDate(date.ToUniversalTime()))
                  .Select(i => i.ToResponse()).ToList();
            var response = await MapImageNames(incidents);
            return response;
        }

        public async Task<IEnumerable<ResponseIncidentDto>> GetAllByCategoryName(string categoryName)
        {
            var incidents = (await _repository.GetAllByCategoryName(categoryName))
                  .Select(i => i.ToResponse()).ToList();
            var response = await MapImageNames(incidents);
            return response;
        }

        public async Task<IEnumerable<ResponseIncidentDto>> GetAllByLocationName(string locationName)
        {
            var incidents = (await _repository.GetAllByLocationName(locationName))
                  .Select(i => i.ToResponse()).ToList();
            var response = await MapImageNames(incidents);
            return response;
        }

        public async Task<IEnumerable<IncidentDto>> GetAll()
        {
            var incidents = await _repository.GetAll();
            return incidents.Select(i => i.ToDto()).ToList();
        }

        public async Task<IEnumerable<ResponseIncidentDto>> MapImageNames(IEnumerable<ResponseIncidentDto> incidents)
        {

            foreach (var incident in incidents)
                incident.Images = await _imageService.GetImageNames(incident.Id) ?? [];
            return incidents;
        }
        public async Task<IEnumerable<ResponseIncidentDto>> GetByCategoryId(int categoryId)
        {
            var incidents = await _repository.GetAllByCategoryId(categoryId);
            var response = await MapImageNames(incidents.Select(i => i.ToResponse()).ToList());
            return response;
        }

        public async Task<ResponseIncidentDto> GetById(int id)
        {
            var incident = await _repository.GetById(id);
            var response = incident == null ?
                   throw new EntityDoesNotExistException($"Incident with id {id} does not exists.") :
                   incident.ToResponse();
            response.Images = await _imageService.GetImageNames(id) ?? [];
            return response;
        }



        public async Task<IncidentDto> Update(int id, IncidentDto incidentDto)
        {
            if (id != incidentDto.Id)
                throw new ArgumentException("The ID in the path does not match the ID in the provided data.");

            if (!await _repository.Exists(i => i.Id == incidentDto.Id))
                throw new EntityDoesNotExistException($"Incident with id {id} does not exists.");

            // Change the tracked entity instead of attaching a second copy of it.
            var incident = (await _repository.GetById(id))!;
            incident.Title = incidentDto.Title;
            incident.Text = incidentDto.Text;
            incident.DateTime = incidentDto.DateTime.ToUniversalTime();
            if (incidentDto.Location is { Id: > 0 })
                incident.LocationId = incidentDto.Location.Id;

            return (await _repository.Update(incident)).ToDto();
        }

        public async Task Approve(ApprovedIncident approvedIncident)
        {
            if (approvedIncident.Categories.Count == 0)
                throw new EntityCanNotBeCreatedException("Incident needs to belong to a category");

            foreach (var categoryName in approvedIncident.Categories)
            {
                if (!await _categoryRepository.Exists(category => category.Name == categoryName))
                    throw new EntityCanNotBeCreatedException("One or more categories do not exist.");
            }

            approvedIncident.DateTime = approvedIncident.DateTime.ToUniversalTime();

            if (approvedIncident.Location == null)
                throw new EntityCanNotBeCreatedException("Incident needs a location");

            var locationExists = await _locationRepository.Exists(l => l.Name == approvedIncident.Location.Name);
            Location location;
            if (!locationExists)
            {
                // A new place gets this database's own id, not the one it had in Management.
                var newLocation = approvedIncident.Location.ToEntity();
                newLocation.Id = 0;
                location = await _locationRepository.Add(newLocation);
            }
            else
            {
                location = (await _locationRepository.Find(l => l.Name == approvedIncident.Location!.Name))!;
            }

            var newIncident = new Incident
            {
                Text = approvedIncident.Text,
                Title = approvedIncident.Title,
                DateTime = approvedIncident.DateTime,
                LocationId = location.Id,
            };
            var incident = await _repository.Add(newIncident);

            // Sequential on purpose: the repositories share one DbContext, which is not thread-safe.
            foreach (var item in approvedIncident.Categories)
            {
                var category = await _categoryRepository.Find(c => c.Name == item);
                await _incidentCategoryRepository.Add(new IncidentCategory
                {
                    IncidentId = incident.Id,
                    CategoryId = category!.Id
                });
            }

            foreach (var image in approvedIncident.ImagesData)
                await _imageService.Add(image, incident.Id);

        }

        public async Task<IEnumerable<SimpleIncident>> GetAllSimple()
        {
            var incidents = await _repository.GetAll();
            return incidents.Select(i => i.ToSimple()).ToList();
        }
    }
}
