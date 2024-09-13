using AutoMapper;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories;
using IncidentAlert.Util;
using System.Linq.Expressions;

namespace IncidentAlert.Services.Implementation
{
    public class CategoryService(IMapper mapper, ICategoryRepository categoryRepository) : ICategoryService
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICategoryRepository _repository = categoryRepository;
        public async Task<CategoryDto> Add(CategoryDto categoryDto)
        {
            bool exists = await _repository.Exists(c => c.Name == categoryDto.Name);
            if (exists)
                throw new InvalidOperationException("Category already exists");

            var category = await _repository.Add(_mapper.Map<CategoryDto, Category>(categoryDto));
            return _mapper.Map<Category, CategoryDto>(category);
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
                throw new EntityCannotBeDeletedException($"Category with ID {id} cannot be deleted. {ex.Message}", ex);
            }
        }
        // TODO delete
        public async Task<IEnumerable<CategoryDto>> Find(Expression<Func<CategoryDto, bool>> predicateDto)
        {
            var predicate = MapPredicate(predicateDto);
            var categories = await _repository.FindAll(predicate);
            return categories.Select(category => _mapper.Map<CategoryDto>(category));
        }

        public async Task<IEnumerable<CategoryDto>> GetAll() => _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDto>>(await _repository.GetAll());

        public async Task<CategoryDto?> GetById(int id)
        {
            var category = await _repository.GetById(id);
            return category == null ?
              throw new EntityDoesNotExistException($"Category with id {id} does not exists.") :
                _mapper.Map<Category, CategoryDto>(category);
        }
        // TODO delete
        public Expression<Func<Category, bool>> MapPredicate(Expression<Func<CategoryDto, bool>> predicateDto)
        {
            var parameter = Expression.Parameter(typeof(Category), "c");
            var visitor = new PredicateVisitor<CategoryDto, Category>(parameter);
            var body = visitor.Visit(predicateDto.Body);
            return Expression.Lambda<Func<Category, bool>>(body, parameter);
        }

        public async Task<CategoryDto> Update(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
                throw new ArgumentException("The ID in the path does not match the ID in the provided data.");

            if (!await _repository.Exists(c => c.Id == categoryDto.Id))
                throw new EntityDoesNotExistException($"Category with id {id} does not exists.");

            var updatedCategory = await _repository.Update(_mapper.Map<CategoryDto, Category>(categoryDto));

            return _mapper.Map<Category, CategoryDto>(updatedCategory);
        }


    }
}
