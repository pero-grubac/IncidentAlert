using AutoMapper;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories;

namespace IncidentAlert.Services.Implementation
{
    public class IncidentService(IMapper mapper, IIncidentRepository incidentRepository,
        ILocationRepository locationRepository, ICategoryRepository categoryRepository,
        IIncidentCategoryRepository incidentCategoryRepository) : IIncidentService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IIncidentRepository _repository = incidentRepository;
        private readonly ILocationRepository _locationRepository = locationRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IIncidentCategoryRepository _incidentCategoryRepository = incidentCategoryRepository;
        public async Task<IncidentDto> Add(IncidentDto incidentDto)
        {
            var invalidCategoryTasks = incidentDto.Categories.Select(async c =>
            {
                var exists = await _categoryRepository.Exists(category => category.Id == c.Id);
                return new { Category = c, Exists = exists };
            }).ToList();

            var invalidCategoriesResults = await Task.WhenAll(invalidCategoryTasks);

            // Filtrirajte one koje ne postoje
            var invalidCategories = invalidCategoriesResults
                .Where(result => !result.Exists)
                .Select(result => result.Category)
                .ToList();

            if (invalidCategories.Any())
            {
                throw new ArgumentException("One or more categories do not exist.");
            };

            incidentDto.DateTime = incidentDto.DateTime.ToUniversalTime();


            var locationExists = await _locationRepository.Exists(l => l.Name == incidentDto.Location.Name);
            Location location;
            if (!locationExists)
            {
                location = await _locationRepository.Add(_mapper.Map<LocationDto, Location>(incidentDto.Location));
            }
            else
            {
                location = (await _locationRepository.Find(l => l.Name == incidentDto.Location.Name))!;
            }


            var newIncident = new Incident
            {
                Text = incidentDto.Text,
                DateTime = incidentDto.DateTime,
                LocationId = location.Id,
            };
            var incident = await _repository.Add(newIncident);

            var incidentCategoriesTasks = incidentDto.Categories.Select(async item =>
            {
                var incidentCategory = new IncidentCategory
                {
                    IncidentId = incident.Id,
                    CategoryId = item.Id
                };
                await _incidentCategoryRepository.Add(incidentCategory);
            });

            await Task.WhenAll(incidentCategoriesTasks);

            var newIncidentDto = _mapper.Map<Incident, IncidentDto>(incident);
            newIncidentDto.Categories = incidentDto.Categories;
            newIncidentDto.Location = _mapper.Map<Location, LocationDto>(location);
            return newIncidentDto;
        }

        public async Task Delete(int id)
        {
            var entity = await _repository.GetById(id) ?? throw new EntityDoesNotExistException($"Incident with ID {id} does not exist.");
            try
            {
                await _incidentCategoryRepository.DeleteAllWithIncidentId(id);
                await _repository.Delete(entity);
            }
            catch (Exception ex)
            {
                throw new EntityCannotBeDeletedException($"Incident with ID {id} cannot be deleted. {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<IncidentDto>> GetAllApprovedIncidentsInDateRange(DateTime startDate, DateTime endDate)
            => _mapper.Map<IEnumerable<Incident>, IEnumerable<IncidentDto>>
                (await _repository.GetAllApprovedIncidentsInDateRange(startDate, endDate));


        public async Task<IEnumerable<IncidentDto>> GetAllApprovedIncidentsOnDate(DateTime date)
            => _mapper.Map<IEnumerable<Incident>, IEnumerable<IncidentDto>>
                (await _repository.GetAllApprovedIncidentsOnDate(date));

        public async Task<IEnumerable<IncidentDto>> GetAllByCategoryName(string categoryName)
            => _mapper.Map<IEnumerable<Incident>, IEnumerable<IncidentDto>>
                (await _repository.GetAllByCategoryName(categoryName));

        public async Task<IEnumerable<IncidentDto>> GetAllByLocationName(string locationName)
             => _mapper.Map<IEnumerable<Incident>, IEnumerable<IncidentDto>>
                (await _repository.GetAllByLocationName(locationName));

        public async Task<IEnumerable<IncidentDto>> GetApproved()
        {
            var incidents = await _repository.GetApproved();
            return _mapper.Map<IEnumerable<Incident>, IEnumerable<IncidentDto>>(incidents);
        }

        public async Task<IEnumerable<IncidentDto>> GetByCategoryId(int categoryId)
        {
            var incidents = await _repository.GetAllByCategoryId(categoryId);
            return _mapper.Map<IEnumerable<Incident>, IEnumerable<IncidentDto>>(incidents);
        }

        public async Task<IncidentDto> GetById(int id)
        {
            var incident = await _repository.GetById(id);
            return incident == null ?
                   throw new EntityDoesNotExistException($"Incident with id {id} does not exists.") :
                   _mapper.Map<Incident, IncidentDto>(incident);
        }

        public async Task<IEnumerable<IncidentDto>> GetRequests()
        {
            var incidents = await _repository.GetRequests();
            return _mapper.Map<IEnumerable<Incident>, IEnumerable<IncidentDto>>(incidents);
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
    }
}
