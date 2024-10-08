using AutoMapper;
using Contracts.Category;
using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using MassTransit;

namespace IncidentAlert.Consumers.CategoryConsumers
{
    public sealed class CategoryUpdatedConsumer(IMapper mapper, ICategoryService categoryService)
        : IConsumer<CategoryUpdateEvent>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICategoryService _categoryService = categoryService;

        public async Task Consume(ConsumeContext<CategoryUpdateEvent> context)
        {
            await _categoryService.Update(_mapper.Map<CategoryUpdateEvent, CategoryDto>(context.Message));
        }
    }
}
