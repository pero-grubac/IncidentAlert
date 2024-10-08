﻿using AutoMapper;
using Contracts.Incident;
using IncidentAlert.Repositories;
using MassTransit;

namespace IncidentAlert.Consumers.IncidentConsumers
{
    public sealed class IncidentApprovedConsumer(IMapper mapper, IIncidentRepository repository)
        : IConsumer<IncidentCreateEvent>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IIncidentRepository _repository = repository;
        public async Task Consume(ConsumeContext<IncidentCreateEvent> context)
        {
            // TODO sacuvaj
            await _repository.GetAll();
        }
    }
}
