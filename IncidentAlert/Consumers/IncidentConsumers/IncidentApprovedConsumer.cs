using Contracts.Incident;
using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using IncidentAlert.Util;
using MassTransit;

namespace IncidentAlert.Consumers.IncidentConsumers
{
    public sealed class IncidentApprovedConsumer(IIncidentService service)
        : IConsumer<IncidentApprovedEvent>
    {
        private readonly IIncidentService _service = service;
        public async Task Consume(ConsumeContext<IncidentApprovedEvent> context)
        {
            await _service.Approve(context.Message.ToApprovedIncident());
        }
    }
}
