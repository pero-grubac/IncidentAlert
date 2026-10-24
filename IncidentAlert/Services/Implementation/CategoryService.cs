using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories;
using IncidentAlert.Util;
using System.Linq.Expressions;

namespace IncidentAlert.Services.Implementation
{
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        private readonly ICategoryRepository _repository = categoryRepository;
        public async Task<CategoryDto> Add(CategoryDto categoryDto)
        {
            bool exists = await _repository.Exists(c => c.Name == categoryDto.Name);
            if (exists)
                throw new InvalidOperationException("Category already exists");

            var category = await _repository.Add(categoryDto.ToEntity());
            return category.ToDto();
        }

        public async Task Delete(int id)
        {
            var entity = await _repository.GetById(id) ?? throw new EntityDoesNotExistException($"Category with ID {id} does not exist.");
            try
            {
                await _repository.Delete(entity);
            }
            catch (Exception ex)
            {
                throw new EntityCannotBeDeletedException($"Category with ID {id} cannot be deleted.", ex);
            }
        }
        // TODO delete
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

            return updatedCategory.ToDto();
        }


    }
}
