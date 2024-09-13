using IncidentAlert.Models;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories
{
    public interface IIncidentRepository
    {
        Task<IEnumerable<Incident>> GetApproved();
        Task<IEnumerable<Incident>> GetRequests();
        Task<Incident?> GetById(int id);
        Task<Incident> Update(Incident incident);
        Task Delete(Incident incident);
        Task<bool> Exists(Expression<Func<Incident, bool>> predicate);
        Task<IEnumerable<Incident>> FindAll(Expression<Func<Incident, bool>> predicate);
    }
}
