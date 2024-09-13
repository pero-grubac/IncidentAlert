using IncidentAlert.Models;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories
{
    public interface IIncidentRepository
    {
        Task<IEnumerable<Incident>> GetApproved();
        Task<IEnumerable<Incident>> GetRequests();
        Task<Incident?> GetById(int id);
        Task Approve(Incident incident, bool isApproved);
        Task<Incident> Update(Incident incident);
        Task Delete(Incident incident);
        Task<bool> Exists(Expression<Func<Incident, bool>> predicate);
        Task<IEnumerable<Incident>> Find(Expression<Func<Incident, bool>> predicate);
    }
}
