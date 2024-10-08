using AutoMapper;
using Contracts.Incident;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using MassTransit;

namespace IncidentAlert_Management.Consumers.IncidentConsumers
{
    public sealed class IncidentCreatedConsumer(IMapper mapper, IIncidentService service)
        : IConsumer<IncidentCreateEvent>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IIncidentService _service = service;
        public async Task Consume(ConsumeContext<IncidentCreateEvent> context)
        {
            var incident = _mapper.Map<IncidentCreateEvent, IncidentDto>(context.Message);
            await _service.Add(incident);
        }
    }
}
