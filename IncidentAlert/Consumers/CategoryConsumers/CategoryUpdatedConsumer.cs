using AutoMapper;
using Contracts.Category;
using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using MassTransit;

namespace IncidentAlert.Consumers.CategoryConsumers
{
    public sealed class CategoryUpdatedConsumer(IMapper mapper, ICategoryService service)
        : IConsumer<CategoryUpdateEvent>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICategoryService _service = service;

        public async Task Consume(ConsumeContext<CategoryUpdateEvent> context)
        {
            await _service.Update(_mapper.Map<CategoryUpdateEvent, CategoryDto>(context.Message));
        }
    }
}
