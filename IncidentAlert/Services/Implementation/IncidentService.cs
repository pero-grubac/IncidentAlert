using AutoMapper;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories;

namespace IncidentAlert.Services.Implementation
{
    public class IncidentService(IMapper mapper, IIncidentRepository incidentRepository) : IIncidentService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IIncidentRepository _repository = incidentRepository;
        public async Task<IncidentDto> Add(IncidentDto incidentDto)
        {
            var incident = await _repository.Add(_mapper.Map<IncidentDto, Incident>(incidentDto));
            return _mapper.Map<Incident, IncidentDto>(incident);
        }

        public async Task Delete(int id)
        {
            var entity = await _repository.GetById(id) ?? throw new EntityDoesNotExistException($"Incident with ID {id} does not exist.");
            try
            {
                await _repository.Delete(entity);
            }
            catch (Exception ex)
            {
                throw new EntityCannotBeDeletedException($"Incident with ID {id} cannot be deleted. {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<IncidentDto>> GetApproved()
        {
            var incidents = await _repository.GetApproved();
            return _mapper.Map<IEnumerable<Incident>, IEnumerable<IncidentDto>>(incidents);
        }

        public async Task<IEnumerable<IncidentDto>> GetByCategoryId(int categoryId)
        {
            var incidents = await _repository.GetByCategoryId(categoryId);
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
