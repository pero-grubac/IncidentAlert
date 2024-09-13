using IncidentAlert.Data;
using IncidentAlert.Models;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories.Implementation
{
    public class IncidentRepository(DataContext dataContext) : IIncidentRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public Task Approve(Incident incident, bool isApproved)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Incident incident)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(Expression<Func<Incident, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Incident>> Find(Expression<Func<Incident, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Incident>> GetApproved()
        {
            throw new NotImplementedException();
        }

        public Task<Incident?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Incident>> GetRequests()
        {
            throw new NotImplementedException();
        }

        public Task<Incident> Update(Incident incident)
        {
            throw new NotImplementedException();
        }
    }
}
