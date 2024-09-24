using IncidentAlert_Management.Data;
using IncidentAlert_Management.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IncidentAlert_Management.Repositories.Implementation
{
    public class CategoryRepository(DataContext dataContext) : ICategoryRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task<Category> Add(Category category)
        {
            await _dataContext.Categories.AddAsync(category);
            await _dataContext.SaveChangesAsync();
            return category;
        }

        public async Task Delete(Category entity)
        {
            _dataContext.Categories.Remove(entity);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<bool> Exists(Expression<Func<Category, bool>> predicate)
            => await _dataContext.Categories.AnyAsync(predicate);

        public async Task<Category?> Find(Expression<Func<Category, bool>> predicate)
            => await _dataContext.Categories.FirstOrDefaultAsync(predicate);

        public async Task<IEnumerable<Category>> FindAll(Expression<Func<Category, bool>> predicate)
            => await _dataContext.Categories.Where(predicate).ToListAsync();

        public async Task<IEnumerable<Category>> GetAll() => await _dataContext.Categories.ToListAsync();

        public async Task<Category?> GetById(int id) => await Find(e => e.Id == id);


        public async Task<Category> Update(Category category)
        {
            _dataContext.Categories.Update(category);
            await _dataContext.SaveChangesAsync();
            return category;
        }
    }
}
