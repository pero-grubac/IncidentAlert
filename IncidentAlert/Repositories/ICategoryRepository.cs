using IncidentAlert.Models;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category?> GetById(int id);
        Task<Category> Add(Category category);
        Task<Category> Update(Category category);
        Task Delete(int id);
        Task<bool> Exists(Expression<Func<Category, bool>> predicate);
        Task<IEnumerable<Category>> Find(Expression<Func<Category, bool>> predicate);
    }
}
