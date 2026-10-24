using IncidentAlert_Management.Exceptions;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Repositories;
using IncidentAlert_Management.Util;

namespace IncidentAlert_Management.Services.Implementation
{
    public class LocationService(ILocationRepository locationRepository) : ILocationService
    {
        private readonly ILocationRepository _repository = locationRepository;
        public async Task<LocationDto> Add(LocationDto locationDto)
        {
            bool exists = await _repository.Exists(c => c.Name == locationDto.Name);
            if (exists)
                throw new InvalidOperationException("Location already exists");

            var location = await _repository.Add(locationDto.ToEntity());
            return location.ToDto();
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

        public async Task<IEnumerable<LocationDto>> GetAll() => (await _repository.GetAll()).Select(l => l.ToDto()).ToList();

        public async Task<LocationDto?> GetById(int id)
        {
            var location = await _repository.GetById(id);
            return location == null ?
              throw new EntityDoesNotExistException($"Location with id {id} does not exists.") :
                location.ToDto();
        }

        public async Task<LocationDto> Update(int id, LocationDto locationDto)
        {
            if (id != locationDto.Id)
                throw new ArgumentException("The ID in the path does not match the ID in the location.");

            if (!await _repository.Exists(c => c.Id == locationDto.Id))
                throw new EntityDoesNotExistException($"Location with id {id} does not exists.");

            var location = await _repository.Update(locationDto.ToEntity());

            return location.ToDto();
        }
    }
}
