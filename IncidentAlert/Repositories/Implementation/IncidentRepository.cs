using IncidentAlert.Data;
using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories.Implementation
{
    public class IncidentRepository(DataContext dataContext) : IIncidentRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task Delete(Incident incident)
        {
            _dataContext.Incidents.Remove(incident);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<bool> Exists(Expression<Func<Incident, bool>> predicate) => await _dataContext.Incidents.AnyAsync(predicate);

        public async Task<IEnumerable<Incident>> FindAll(Expression<Func<Incident, bool>> predicate) => await _dataContext.Incidents.Where(predicate).ToListAsync();

        public async Task<IEnumerable<Incident>> GetApproved() => await FindAll(i => i.IsApproved == true);

        public Task<Incident?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Incident>> GetRequests() => await FindAll(i => i.IsApproved == false);

        public Task<Incident> Update(Incident incident)
        {
            throw new NotImplementedException();
        }
    }
}
