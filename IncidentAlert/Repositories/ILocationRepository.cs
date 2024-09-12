using IncidentAlert.Models;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(int id);
        Task<Location> AddAsync(Location location);
        Task<Location> UpdateAsync(Location location);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<Location, bool>> predicate);
        Task<IEnumerable<Location>> FindAsync(Expression<Func<Location, bool>> predicate);
    }
}
