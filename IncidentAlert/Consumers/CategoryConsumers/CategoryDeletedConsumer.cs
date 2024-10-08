using Contracts.Category;
using IncidentAlert.Services;
using MassTransit;

namespace IncidentAlert.Consumers.CategoryConsumers
{
    public sealed class CategoryDeletedConsumer(ICategoryService service)
        : IConsumer<CategoryDeleteEvent>
    {
        private readonly ICategoryService _service = service;

        public async Task Consume(ConsumeContext<CategoryDeleteEvent> context)
        {
            await _service.Delete(context.Message.Id);
        }
    }
}
