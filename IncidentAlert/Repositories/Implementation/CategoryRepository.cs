﻿using IncidentAlert.Data;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IncidentAlert.Repositories.Implementation
{
    public class CategoryRepository(DataContext dataContext) : ICategoryRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task<Category> AddAsync(Category category)
        {
            await _dataContext.Categories.AddAsync(category);
            await _dataContext.SaveChangesAsync();
            return category;
        }

        public async Task DeleteAsync(int id)
        {
            // TODO prebaci u servis, a da je repo samo za perzistenciju
            var entity = await _dataContext.Categories.FirstOrDefaultAsync(x => x.Id == id) ?? throw new EntityDoesNotExistException($"Entity with ID {id} does not exist.");
            try
            {
                _dataContext.Categories.Remove(entity);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new EntityCannotBeDeletedException($"Entity with ID {id} cannot be deleted. {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<Category, bool>> predicate) => await _dataContext.Categories.AnyAsync(predicate);

        public async Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate) => await _dataContext.Categories.Where(predicate).ToListAsync();

        public async Task<IEnumerable<Category>> GetAllAsync() => await _dataContext.Categories.ToListAsync();

        public async Task<Category?> GetByIdAsync(int id) => await _dataContext.Categories.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<Category> UpdateAsync(Category category)
        {
            _dataContext.Categories.Update(category);
            await _dataContext.SaveChangesAsync();
            return category;
        }
    }
}
