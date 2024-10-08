using AutoMapper;
using Contracts.Incident;
using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using MassTransit;

namespace IncidentAlert.Consumers.IncidentConsumers
{
    public sealed class IncidentApprovedConsumer(IMapper mapper, IIncidentService service)
        : IConsumer<IncidentApprovedEvent>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IIncidentService _service = service;
        public async Task Consume(ConsumeContext<IncidentApprovedEvent> context)
        {
            await _service.Approve(_mapper.Map<IncidentApprovedEvent, ApprovedIncident>(context.Message));
        }
    }
}
