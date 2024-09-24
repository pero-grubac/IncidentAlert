using IncidentAlert.Models;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories
{
    public interface IIncidentCategoryRepository
    {
        Task Add(IncidentCategory incidentCategory);
        Task Delete(IncidentCategory incidentCategory);
        Task<bool> Exists(Expression<Func<IncidentCategory, bool>> predicate);
        Task<ICollection<IncidentCategory>> FindAll(Expression<Func<IncidentCategory, bool>> predicate);
        Task<ICollection<IncidentCategory>> FindAllByIncidentId(int incidentId);
        Task DeleteAllWithIncidentId(int incidentId);
    }
}
