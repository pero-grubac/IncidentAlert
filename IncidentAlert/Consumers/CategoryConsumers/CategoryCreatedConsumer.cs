using AutoMapper;
using Contracts.Category;
using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using MassTransit;

namespace IncidentAlert.Consumers.CategoryConsumers
{
    public sealed class CategoryCreatedConsumer(IMapper mapper, ICategoryService service)
        : IConsumer<CategoryCreatedEvent>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICategoryService _service = service;

        public async Task Consume(ConsumeContext<CategoryCreatedEvent> context)
        {
            await _service.Add(_mapper.Map<CategoryCreatedEvent, CategoryDto>(context.Message));
        }
    }
}
