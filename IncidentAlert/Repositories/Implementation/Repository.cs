using IncidentAlert.Data;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories.Implementation
{
    public class Repository<TEntity>(DataContext context, DbSet<TEntity> dbSet) : IRepository<TEntity, int> where TEntity : BaseEntity<int>
    {
        private readonly DataContext _dataContext = context;
        private readonly DbSet<TEntity> _dbSet = dbSet;

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _dataContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id) ?? throw new EntityDoesNotExistException($"Entity with ID {id} does not exist.");
            try
            {
                _dbSet.Remove(entity);
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new EntityCannotBeDeletedException($"Entity with ID {id} cannot be deleted. {ex.Message}", ex);
            }

        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate) => await _dbSet.AnyAsync(predicate);

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<TEntity?> GetByIdAsync(int id) => await _dbSet.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _dataContext.SaveChangesAsync();
            return entity;
        }
    }
}
