using IncidentAlert.Data;
using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories.Implementation
{
    public class IncidentRepository(DataContext dataContext) : IIncidentRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task<Incident> Add(Incident incident)
        {
            await _dataContext.Incidents.AddAsync(incident);
            await _dataContext.SaveChangesAsync();
            return incident;
        }

        public async Task Delete(Incident incident)
        {
            _dataContext.Incidents.Remove(incident);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<bool> Exists(Expression<Func<Incident, bool>> predicate)
            => await _dataContext.Incidents.AnyAsync(predicate);

        public async Task<Incident?> Find(Expression<Func<Incident, bool>> predicate)
            => await _dataContext.Incidents.FirstOrDefaultAsync(predicate);

        public async Task<IEnumerable<Incident>> FindAll(Expression<Func<Incident, bool>> predicate)
            => await _dataContext.Incidents.Where(predicate)
                        .Include(i => i.Location)
                        .Include(i => i.IncidentCategories)
                            .ThenInclude(ic => ic.Category)
                        .OrderByDescending(i => i.DateTime)
                        .ToListAsync();

        public async Task<IEnumerable<Incident>> GetApproved()
            => await FindAll(i => i.IsApproved == true);

        public async Task<IEnumerable<Incident>> GetAllByCategoryId(int categoryId)
            => await _dataContext.IncidentCategories
                .Where(ic => ic.CategoryId == categoryId)
                .Select(ic => ic.Incident).OrderByDescending(i => i.DateTime).ToListAsync();

        public async Task<Incident?> GetById(int id) => await Find(i => i.Id == id);

        public async Task<IEnumerable<Incident>> GetRequests()
            => await FindAll(i => i.IsApproved == false);

        public async Task<Incident> Update(Incident incident)
        {
            _dataContext.Incidents.Update(incident);
            await _dataContext.AddRangeAsync(incident);
            return incident;
        }

        public async Task<IEnumerable<Incident>> GetAllByCategoryName(string categoryName)
            => await _dataContext.IncidentCategories
                        .Where(ic => ic.Category.Name == categoryName && ic.Incident.IsApproved == true)
                        .Select(ic => ic.Incident)
                        .OrderByDescending(i => i.DateTime).ToListAsync();

        public async Task<IEnumerable<Incident>> GetAllOnDate(DateTime date)
            => await FindAll(i => i.IsApproved == true && i.DateTime.Date == date.Date);

        public async Task<IEnumerable<Incident>> GetAllInDateRange(DateTime startDate, DateTime endDate)
            => await FindAll(i => i.IsApproved == true && i.DateTime.Date >= startDate.Date && i.DateTime.Date <= endDate.Date);

        public async Task<IEnumerable<Incident>> GetAllByLocationName(string locationName)
            => await FindAll(i => i.IsApproved == true && i.Location.Name == locationName);
    }
}
