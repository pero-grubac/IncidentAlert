using Contracts.Category;
using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using IncidentAlert.Util;
using MassTransit;

namespace IncidentAlert.Consumers.CategoryConsumers
{
    public sealed class CategoryUpdatedConsumer(ICategoryService service)
        : IConsumer<CategoryUpdateEvent>
    {
        private readonly ICategoryService _service = service;

        public async Task Consume(ConsumeContext<CategoryUpdateEvent> context)
        {
            await _service.Update(context.Message.ToDto());
        }
    }
}
