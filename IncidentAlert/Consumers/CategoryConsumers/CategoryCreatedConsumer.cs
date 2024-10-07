using AutoMapper;
using Contracts.Category;
using IncidentAlert.Models;
using IncidentAlert.Repositories;
using MassTransit;

namespace IncidentAlert.Consumers.CategoryConsumers
{
    public sealed class CategoryCreatedConsumer(IMapper mapper, ICategoryRepository repository)
        : IConsumer<CategoryCreatedEvent>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICategoryRepository _repository = repository;

        public async Task Consume(ConsumeContext<CategoryCreatedEvent> context)
        {
            bool exists = await _repository.Exists(c => c.Name == context.Message.Name);
            if (!exists)
                await _repository.Add(_mapper.Map<CategoryCreatedEvent, Category>(context.Message));
        }
    }
}
