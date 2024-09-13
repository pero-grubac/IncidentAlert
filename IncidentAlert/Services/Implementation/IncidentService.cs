using AutoMapper;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories;

namespace IncidentAlert.Services.Implementation
{
    public class IncidentService(IMapper mapper, IIncidentRepository incidentRepository) : IIncidentService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IIncidentRepository _repositroy = incidentRepository;
        public async Task<IncidentDto> Add(IncidentDto incidentDto)
        {
            var incident = await _repositroy.Add(_mapper.Map<IncidentDto, Incident>(incidentDto));
            return _mapper.Map<Incident, IncidentDto>(incident);
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IncidentDto>> GetApproved()
        {
            throw new NotImplementedException();
        }


        public Task<IncidentDto> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IncidentDto>> GetRequests()
        {
            throw new NotImplementedException();
        }

        public Task<IncidentDto> Update(int id, IncidentDto incident)
        {
            throw new NotImplementedException();
        }
    }
}
