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

        public async Task<bool> Exists(Expression<Func<IncidentCategory, bool>> predicate) => await _dataContext.IncidentCategories.AnyAsync(predicate);
    }
}
