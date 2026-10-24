using Contracts.Category;
using IncidentAlert_Management.Exceptions;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Repositories;
using IncidentAlert_Management.Util;
using MassTransit;
using System.Linq.Expressions;

namespace IncidentAlert_Management.Services.Implementation
{
    public class CategoryService(ICategoryRepository categoryRepository,
        IPublishEndpoint publishEndpoint) : ICategoryService
    {
        private readonly ICategoryRepository _repository = categoryRepository;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        public async Task<CategoryDto> Add(string name)
        {
            bool exists = await _repository.Exists(c => c.Name == name);
            if (exists)
                throw new InvalidOperationException("Category already exists");

            var category = await _repository.Add(new Category
            {
                Name = name,
            });

            await _publishEndpoint.Publish(category.ToCreatedEvent());

            return category.ToDto();
        }

        public async Task Delete(int id)
        {
            var entity = await _repository.GetById(id) ?? throw new EntityDoesNotExistException($"Category with ID {id} does not exist.");
            try
            {
                await _repository.Delete(entity);
                await _publishEndpoint.Publish(entity.ToDeleteEvent());
            }
            catch (Exception ex)
            {
                throw new EntityCannotBeDeletedException($"Category with ID {id} cannot be deleted.", ex);
            }
        }

        public async Task<IEnumerable<CategoryDto>> Find(Expression<Func<CategoryDto, bool>> predicateDto)
        {
            var predicate = MapPredicate(predicateDto);
            var categories = await _repository.FindAll(predicate);
            return categories.Select(category => category.ToDto());
        }

        public async Task<IEnumerable<CategoryDto>> GetAll() => (await _repository.GetAll()).Select(c => c.ToDto()).ToList();

        public async Task<CategoryDto?> GetById(int id)
        {
            var category = await _repository.GetById(id);
            return category == null ?
              throw new EntityDoesNotExistException($"Category with id {id} does not exists.") :
                category.ToDto();
        }
        // TODO delete
        public Expression<Func<Category, bool>> MapPredicate(Expression<Func<CategoryDto, bool>> predicateDto)
        {
            var parameter = Expression.Parameter(typeof(Category), "c");
            var visitor = new PredicateVisitor<CategoryDto, Category>(parameter);
            var body = visitor.Visit(predicateDto.Body);
            return Expression.Lambda<Func<Category, bool>>(body, parameter);
        }

        public async Task<CategoryDto> Update(CategoryDto categoryDto)
        {

            if (!await _repository.Exists(c => c.Id == categoryDto.Id))
                throw new EntityDoesNotExistException($"Category with id {categoryDto.Id} does not exists.");

            var updatedCategory = await _repository.Update(categoryDto.ToEntity());

            await _publishEndpoint.Publish(updatedCategory.ToUpdateEvent());

            return updatedCategory.ToDto();
        }


    }
}
