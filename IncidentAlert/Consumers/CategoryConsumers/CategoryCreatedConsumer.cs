using Contracts.Category;
using IncidentAlert.Models.Dto;
using IncidentAlert.Services;
using IncidentAlert.Util;
using MassTransit;

namespace IncidentAlert.Consumers.CategoryConsumers
{
    public sealed class CategoryCreatedConsumer(ICategoryService service)
        : IConsumer<CategoryCreatedEvent>
    {
        private readonly ICategoryService _service = service;

        public async Task Consume(ConsumeContext<CategoryCreatedEvent> context)
        {
            await _service.Add(context.Message.ToDto());
        }
    }
}
