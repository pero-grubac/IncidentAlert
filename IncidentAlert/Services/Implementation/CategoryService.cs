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
        public async Task<CategoryDto> AddAsync(CategoryDto entity)
        {
            bool exists = await _repository.Exists(c => c.Name == entity.Name);
            if (exists)
                throw new InvalidOperationException("Category already exists");

            var category = await _repository.Add(_mapper.Map<CategoryDto, Category>(entity));
            return _mapper.Map<Category, CategoryDto>(category);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.Delete(id);
        }

        public async Task<IEnumerable<CategoryDto>> FindAsync(Expression<Func<CategoryDto, bool>> predicateDto)
        {
            var predicate = MapPredicate(predicateDto);
            var categories = await _repository.Find(predicate);
            return categories.Select(category => _mapper.Map<CategoryDto>(category));
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync() => _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDto>>(await _repository.GetAll());

        public async Task<CategoryDto?> GetAsync(int id)
        {
            var category = await _repository.GetById(id);
            return category == null ?
              throw new EntityDoesNotExistException($"Category with id {id} does not exists.") :
                _mapper.Map<Category, CategoryDto>(category);
        }

        public Expression<Func<Category, bool>> MapPredicate(Expression<Func<CategoryDto, bool>> predicateDto)
        {
            var parameter = Expression.Parameter(typeof(Category), "c");
            var visitor = new PredicateVisitor<CategoryDto, Category>(parameter);
            var body = visitor.Visit(predicateDto.Body);
            return Expression.Lambda<Func<Category, bool>>(body, parameter);
        }

        public async Task<CategoryDto> UpdateAsync(int id, CategoryDto entity)
        {
            if (id != entity.Id)
                throw new ArgumentException("The ID in the path does not match the ID in the provided data.");

            if (!await _repository.Exists(c => c.Id == entity.Id))
                throw new EntityDoesNotExistException($"Category with id {id} does not exists.");

            var updatedCategory = await _repository.Update(_mapper.Map<CategoryDto, Category>(entity));

            return _mapper.Map<Category, CategoryDto>(updatedCategory);
        }


    }
}
