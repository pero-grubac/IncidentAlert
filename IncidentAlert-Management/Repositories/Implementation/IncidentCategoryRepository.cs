using IncidentAlert.Data;
using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories.Implementation
{
    public class IncidentCategoryRepository(DataContext dataContext) : IIncidentCategoryRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task Add(IncidentCategory incidentCategory)
        {
            await _dataContext.IncidentCategories.AddAsync(incidentCategory);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Delete(IncidentCategory incidentCategory)
        {
            _dataContext.IncidentCategories.Remove(incidentCategory);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteAllWithIncidentId(int incidentId)
        {
            var incidentCategories = await FindAllByIncidentId(incidentId);
            if (incidentCategories.Any())
            {
                _dataContext.IncidentCategories.RemoveRange(incidentCategories);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(Expression<Func<IncidentCategory, bool>> predicate)
            => await _dataContext.IncidentCategories.AnyAsync(predicate);

        public async Task<ICollection<IncidentCategory>> FindAll(Expression<Func<IncidentCategory, bool>> predicate)
            => await _dataContext.IncidentCategories.Where(predicate).ToListAsync();

        public async Task<ICollection<IncidentCategory>> FindAllByIncidentId(int incidentId)
            => await FindAll(ic => ic.IncidentId == incidentId);
    }
}
