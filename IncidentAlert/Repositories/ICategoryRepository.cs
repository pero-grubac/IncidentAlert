using IncidentAlert.Models;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<Category, bool>> predicate);
        Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate);
    }
}
