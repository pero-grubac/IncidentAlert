using IncidentAlert_Management.Models;
using System.Linq.Expressions;

namespace IncidentAlert_Management.Repositories
{
    public interface IIncidentRepository
    {
        Task<IEnumerable<Incident>> GetAll();
        Task<IEnumerable<Incident>> GetAllByCategoryId(int categoryId);

        Task<Incident?> GetById(int id);
        Task<Incident> Update(Incident incident);
        Task<Incident> Add(Incident incident);
        Task Delete(Incident incident);
        Task<bool> Exists(Expression<Func<Incident, bool>> predicate);
        Task<IEnumerable<Incident>> FindAll(Expression<Func<Incident, bool>> predicate);
        Task<Incident?> Find(Expression<Func<Incident, bool>> predicate);
        Task<IEnumerable<Incident>> GetAllByCategoryName(string categoryName);
        Task<IEnumerable<Incident>> GetAllOnDate(DateTime date);
        Task<IEnumerable<Incident>> GetAllInDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Incident>> GetAllByLocationName(string locationName);

    }
}
