using IncidentAlert.Data;
using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories.Implementation
{
    public class LocationRepository(DataContext dataContext) : ILocationRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task<Location> Add(Location location)
        {
            await _dataContext.Locations.AddAsync(location);
            await _dataContext.SaveChangesAsync();
            return location;
        }

        public async Task Delete(Location entity)
        {
            _dataContext.Locations.Remove(entity);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<bool> Exists(Expression<Func<Location, bool>> predicate) => await _dataContext.Locations.AnyAsync(predicate);

        public async Task<Location?> Find(Expression<Func<Location, bool>> predicate) => await _dataContext.Locations.FirstOrDefaultAsync(predicate);

        public async Task<IEnumerable<Location>> FindAll(Expression<Func<Location, bool>> predicate) => await _dataContext.Locations.Where(predicate).ToListAsync();


        public async Task<IEnumerable<Location>> GetAll() => await _dataContext.Locations.ToListAsync();

        public async Task<Location?> GetById(int id) => await Find(e => e.Id == id);

        public async Task<Location> Update(Location location)
        {
            _dataContext.Locations.Update(location);
            await _dataContext.SaveChangesAsync();
            return location;
        }
    }
}
