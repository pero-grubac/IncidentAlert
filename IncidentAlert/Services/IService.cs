﻿using System.Linq.Expressions;

namespace IncidentAlert.Services
{
    public interface IService<TDto, T>
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> GetAsync(int id);
        Task<TDto> AddAsync(TDto entity);
        Task<TDto> UpdateAsync(int id, TDto entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<TDto>> FindAsync(Expression<Func<TDto, bool>> predicateDto);
        Expression<Func<T, bool>> MapPredicate(Expression<Func<TDto, bool>> predicateDto);
    }
}
