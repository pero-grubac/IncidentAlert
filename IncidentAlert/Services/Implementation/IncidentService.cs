﻿using AutoMapper;
using Contracts.Incident;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories;
using MassTransit;

namespace IncidentAlert.Services.Implementation
{
    public class IncidentService(IMapper mapper, IIncidentRepository incidentRepository,
        ILocationRepository locationRepository, ICategoryRepository categoryRepository,
        IIncidentCategoryRepository incidentCategoryRepository, IImageService imageService,
        IPublishEndpoint publishEndpoint) : IIncidentService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IIncidentRepository _repository = incidentRepository;
        private readonly ILocationRepository _locationRepository = locationRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IIncidentCategoryRepository _incidentCategoryRepository = incidentCategoryRepository;
        private readonly IImageService _imageService = imageService;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        public async Task Add(IncidentDto incidentDto)
        {
            var incidentEvent = _mapper.Map<IncidentDto, IncidentCreateEvent>(incidentDto);
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
                await Task.WhenAll(images.Select(async item => await _imageService.Delete(item.Id)).ToList());
                await _repository.Delete(entity);

            }
            catch (Exception ex)
            {
                throw new EntityCannotBeDeletedException($"Incident with ID {id} cannot be deleted. {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ResponseIncidentDto>> GetAllInDateRange(DateTime startDate, DateTime endDate)
        {
            var incidents = _mapper.Map<IEnumerable<Incident>, IEnumerable<ResponseIncidentDto>>
                  (await _repository.GetAllInDateRange(startDate.ToUniversalTime(), endDate.ToUniversalTime()));
            var response = await MapImageNames(incidents);
            return response;
        }


        public async Task<IEnumerable<ResponseIncidentDto>> GetAllOnDate(DateTime date)
        {
            var incidents = _mapper.Map<IEnumerable<Incident>, IEnumerable<ResponseIncidentDto>>
                  (await _repository.GetAllOnDate(date.ToUniversalTime()));
            var response = await MapImageNames(incidents);
            return response;
        }

        public async Task<IEnumerable<ResponseIncidentDto>> GetAllByCategoryName(string categoryName)
        {
            var incidents = _mapper.Map<IEnumerable<Incident>, IEnumerable<ResponseIncidentDto>>
                  (await _repository.GetAllByCategoryName(categoryName));
            var response = await MapImageNames(incidents);
            return response;
        }

        public async Task<IEnumerable<ResponseIncidentDto>> GetAllByLocationName(string locationName)
        {
            var incidents = _mapper.Map<IEnumerable<Incident>, IEnumerable<ResponseIncidentDto>>
                  (await _repository.GetAllByLocationName(locationName));
            var response = await MapImageNames(incidents);
            return response;
        }

        public async Task<IEnumerable<IncidentDto>> GetAll()
        {
            var incidents = await _repository.GetAll();
            return _mapper.Map<IEnumerable<Incident>, IEnumerable<IncidentDto>>(incidents);
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
            var response = await MapImageNames(_mapper.Map<IEnumerable<Incident>, IEnumerable<ResponseIncidentDto>>(incidents));
            return response;
        }

        public async Task<ResponseIncidentDto> GetById(int id)
        {
            var incident = await _repository.GetById(id);
            var response = incident == null ?
                   throw new EntityDoesNotExistException($"Incident with id {id} does not exists.") :
                   _mapper.Map<Incident, ResponseIncidentDto>(incident);
            response.Images = await _imageService.GetImageNames(id) ?? [];
            return response;
        }



        public async Task<IncidentDto> Update(int id, IncidentDto incidentDto)
        {
            if (id != incidentDto.Id)
                throw new ArgumentException("The ID in the path does not match the ID in the provided data.");

            if (!await _repository.Exists(i => i.Id == incidentDto.Id))
                throw new EntityDoesNotExistException($"Incident with id {id} does not exists.");

            var incident = await _repository.Update(_mapper.Map<IncidentDto, Incident>(incidentDto));
            return _mapper.Map<Incident, IncidentDto>(incident);
        }

        public async Task Approve(ApprovedIncident approvedIncident)
        {
            if (approvedIncident.Categories.Count == 0)
                throw new EntityCanNotBeCreatedException("Incident needs to belong to a category");

            var invalidCategoryTasks = approvedIncident.Categories.Select(async c =>
            {
                var exists = await _categoryRepository.Exists(category => category.Name == c);
                return new { Category = c, Exists = exists };
            }).ToList();

            var invalidCategoriesResults = await Task.WhenAll(invalidCategoryTasks);

            // Filtrirajte one koje ne postoje
            var invalidCategories = invalidCategoriesResults
                .Where(result => !result.Exists)
                .Select(result => result.Category)
                .ToList();

            if (invalidCategories.Count != 0)
            {
                throw new EntityCanNotBeCreatedException("One or more categories do not exist.");
            };

            approvedIncident.DateTime = approvedIncident.DateTime.ToUniversalTime();

            if (approvedIncident.Location == null)
                throw new EntityCanNotBeCreatedException("Incident needs a location");

            var locationExists = await _locationRepository.Exists(l => l.Name == approvedIncident.Location.Name);
            Location location;
            if (!locationExists)
            {
                location = await _locationRepository.Add(_mapper.Map<LocationDto, Location>(approvedIncident.Location!));
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

            var incidentCategoriesTasks = approvedIncident.Categories.Select(async item =>
            {
                var category = await _categoryRepository.Find(c => c.Name == item);

                var incidentCategory = new IncidentCategory
                {
                    IncidentId = incident.Id,
                    CategoryId = category!.Id
                };
                await _incidentCategoryRepository.Add(incidentCategory);
            });

            await Task.WhenAll(incidentCategoriesTasks);

            if (approvedIncident.ImagesData.Count > 0)
                await Task.WhenAll(approvedIncident.ImagesData.Select(async item => await _imageService.Add(item, incident.Id)).ToList());

        }

        public async Task<IEnumerable<SimpleIncident>> GetAllSimple()
        {
            var incidents = await _repository.GetAll();
            return _mapper.Map<IEnumerable<Incident>, IEnumerable<SimpleIncident>>(incidents);
        }
    }
}
