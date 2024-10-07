using AutoMapper;
using Contracts.Incident;
using IncidentAlert_Management.Repositories;
using MassTransit;

namespace IncidentAlert_Management.Consumers.IncidentConsumers
{
    public sealed class IncidentCreatedConsumer(IMapper mapper, IIncidentRepository repository)
        : IConsumer<IncidentCreateEvent>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IIncidentRepository _repository = repository;

        public Task Consume(ConsumeContext<IncidentCreateEvent> context)
        {
            // TODO sacuvaj
            throw new NotImplementedException();
        }
    }
}
