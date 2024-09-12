using IncidentAlert.Data;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories.Implementation
{
    public class LocationRepository(DataContext dataContext) : ILocationRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task<Location> AddAsync(Location location)
        {
            await _dataContext.Locations.AddAsync(location);
            await _dataContext.SaveChangesAsync();
            return location;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dataContext.Locations.FirstOrDefaultAsync(x => x.Id == id) ?? throw new EntityDoesNotExistException($"Entity with ID {id} does not exist.");
            try
            {
                _dataContext.Locations.Remove(entity);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new EntityCannotBeDeletedException($"Entity with ID {id} cannot be deleted. {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<Location, bool>> predicate) => await _dataContext.Locations.AnyAsync(predicate);

        public async Task<IEnumerable<Location>> FindAsync(Expression<Func<Location, bool>> predicate) => await _dataContext.Locations.Where(predicate).ToListAsync();


        public async Task<IEnumerable<Location>> GetAllAsync() => await _dataContext.Locations.ToListAsync();

        public async Task<Location?> GetByIdAsync(int id) => await _dataContext.Locations.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<Location> UpdateAsync(Location location)
        {
            _dataContext.Locations.Update(location);
            await _dataContext.SaveChangesAsync();
            return location;
        }
    }
}
