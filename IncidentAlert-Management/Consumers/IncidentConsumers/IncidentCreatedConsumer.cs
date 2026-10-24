using Contracts.Incident;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Services;
using IncidentAlert_Management.Util;
using MassTransit;

namespace IncidentAlert_Management.Consumers.IncidentConsumers
{
    public sealed class IncidentCreatedConsumer(IIncidentService service)
        : IConsumer<IncidentCreateEvent>
    {
        private readonly IIncidentService _service = service;
        public async Task Consume(ConsumeContext<IncidentCreateEvent> context)
        {
            var incident = context.Message.ToDto();
            await _service.Add(incident);
        }
    }
}
