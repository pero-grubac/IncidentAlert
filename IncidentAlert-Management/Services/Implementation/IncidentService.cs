using Contracts.Incident;
using IncidentAlert_Management.Exceptions;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Repositories;
using IncidentAlert_Management.Util;
using MassTransit;

namespace IncidentAlert_Management.Services.Implementation
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
            if (incidentDto.Categories.Count == 0)
            {
                throw new EntityCanNotBeCreatedException("Incident needs to belong to a category");
            };

            foreach (var categoryName in incidentDto.Categories)
            {
                if (!await _categoryRepository.Exists(category => category.Name == categoryName))
                    throw new EntityCanNotBeCreatedException("One or more categories do not exist.");
            }

            incidentDto.DateTime = incidentDto.DateTime.ToUniversalTime();


            var locationExists = await _locationRepository.Exists(l => l.Name == incidentDto.Location.Name);
            Location location;
            if (!locationExists)
            {
                location = await _locationRepository.Add(NewLocation(incidentDto.Location));
            }
            else
            {
                location = (await _locationRepository.Find(l => l.Name == incidentDto.Location.Name))!;
            }


            var newIncident = new Incident
            {
                Text = incidentDto.Text,
                Title = incidentDto.Title,
                DateTime = incidentDto.DateTime,
                LocationId = location.Id,
            };
            var incident = await _repository.Add(newIncident);

            // Sequential on purpose: the repositories share one DbContext, which is not thread-safe.
            foreach (var item in incidentDto.Categories)
            {
                var category = await _categoryRepository.Find(c => c.Name == item);
                await _incidentCategoryRepository.Add(new IncidentCategory
                {
                    IncidentId = incident.Id,
                    CategoryId = category!.Id
                });
            }

            foreach (var image in incidentDto.ImagesData)
                await _imageService.Add(image, incident.Id);

        }

        // A new place gets its own id here, whatever the id it had in the IncidentAlert service.
        private static Location NewLocation(LocationDto dto)
        {
            var location = dto.ToEntity();
            location.Id = 0;
            return location;
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

        public async Task<IEnumerable<ResponseIncidentDto>> GetAll()
        {
            var incidents = (await _repository.GetAll())
                .Select(i => i.ToResponse()).ToList();
            var response = await MapImageNames(incidents);
            return response;
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

        public async Task<ResponseIncidentDto> ChnageStatus(int id, string status)
        {
            if (!Enum.TryParse(typeof(StatusEnum), status, true, out var statusEnumObj)
                || !Enum.IsDefined(typeof(StatusEnum), statusEnumObj))
                throw new ArgumentException($"Status '{status}' is not valid.");

            var incident = await _repository.GetById(id) ?? throw new EntityDoesNotExistException($"Incident with id {id} doesnt exist");

            var statusEnum = (StatusEnum)statusEnumObj;
            // Accepting goes through Approve, which also publishes the incident.
            if (statusEnum == StatusEnum.ACCEPTED)
                throw new ArgumentException("Use Approve to publish an incident.");

            incident.Status = statusEnum;
            await _repository.Update(incident);

            return incident.ToResponse();
        }

        public async Task Approve(int id)
        {
            if (!await _repository.Exists(i => i.Id == id))
                throw new EntityDoesNotExistException($"Incident with id {id} does not exists.");

            var incident = (await _repository.GetById(id))!;
            var approvedIncident = incident.ToApprovedEvent();
            approvedIncident.ImagesData = (await _imageService.GetImageData(id)).Select(i => i.ToMessage()).ToList();

            await _publishEndpoint.Publish(approvedIncident);

            await Delete(id);
        }
    }
}
