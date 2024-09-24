using IncidentAlert_Management.Models;
using System.Linq.Expressions;

namespace IncidentAlert_Management.Repositories
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAll();
        Task<Location?> GetById(int id);
        Task<Location> Add(Location location);
        Task<Location> Update(Location location);
        Task Delete(Location entity);
        Task<bool> Exists(Expression<Func<Location, bool>> predicate);
        Task<IEnumerable<Location>> FindAll(Expression<Func<Location, bool>> predicate);
        Task<Location?> Find(Expression<Func<Location, bool>> predicate);
    }
}
