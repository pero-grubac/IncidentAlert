using AutoMapper;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories;

namespace IncidentAlert.Services.Implementation
{
    public class LocationService(IMapper mapper, ILocationRepository locationRepository) : ILocationService
    {
        private readonly IMapper _mapper = mapper;
        private readonly ILocationRepository _repository = locationRepository;
        public async Task<LocationDto> Add(LocationDto locationDto)
        {
            bool exists = await _repository.Exists(c => c.Name == locationDto.Name);
            if (exists)
                throw new InvalidOperationException("Location already exists");

            var location = await _repository.Add(_mapper.Map<LocationDto, Location>(locationDto));
            return _mapper.Map<Location, LocationDto>(location);
        }

        public async Task Delete(int id)
        {
            var location = await _repository.GetById(id) ?? throw new EntityDoesNotExistException($"Location with ID {id} does not exist.");

            try
            {
                await _repository.Delete(location);
            }
            catch (Exception ex)
            {
                throw new EntityCannotBeDeletedException($"Location with ID {id} cannot be deleted.", ex);
            }
        }

        public async Task<IEnumerable<LocationDto>> GetAll() => _mapper.Map<IEnumerable<Location>, IEnumerable<LocationDto>>(await _repository.GetAll());

        public async Task<LocationDto?> GetById(int id)
        {
            var location = await _repository.GetById(id);
            return location == null ?
              throw new EntityDoesNotExistException($"Location with id {id} does not exists.") :
                _mapper.Map<Location, LocationDto>(location);
        }

        public async Task<LocationDto> Update(int id, LocationDto locationDto)
        {
            if (id != locationDto.Id)
                throw new ArgumentException("The ID in the path does not match the ID in the location.");

            if (!await _repository.Exists(c => c.Id == locationDto.Id))
                throw new EntityDoesNotExistException($"Location with id {id} does not exists.");

            var location = await _repository.Update(_mapper.Map<LocationDto, Location>(locationDto));

            return _mapper.Map<Location, LocationDto>(location);
        }
    }
}
