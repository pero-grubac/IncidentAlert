using IncidentAlert_Management.Models;
using System.Linq.Expressions;

namespace IncidentAlert_Management.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category?> GetById(int id);
        Task<Category> Add(Category category);
        Task<Category> Update(Category category);
        Task Delete(Category entity);
        Task<bool> Exists(Expression<Func<Category, bool>> predicate);
        Task<IEnumerable<Category>> FindAll(Expression<Func<Category, bool>> predicate);
        Task<Category?> Find(Expression<Func<Category, bool>> predicate);

    }
}
