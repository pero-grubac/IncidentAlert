﻿using IncidentAlert_Management.Models;
using System.Linq.Expressions;

namespace IncidentAlert_Management.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAll();
        Task<ApplicationUser?> GetByUsername(string username);
        Task<ApplicationUser?> GetById(string id);
        Task<ApplicationUser?> GetByGoogleId(string googleId);

        Task<ApplicationUser?> GetByEmail(string email);
        Task<RoleEnum> GetRoleByUsername(string username);

        Task Add(ApplicationUser user);
        Task Update(ApplicationUser user);
        Task DeleteByUsername(string username);

        Task<bool> Exists(Expression<Func<ApplicationUser, bool>> predicate);
        Task<IEnumerable<ApplicationUser>> FindAll(Expression<Func<ApplicationUser, bool>> predicate);
        Task<ApplicationUser?> Find(Expression<Func<ApplicationUser, bool>> predicate);
    }
}
