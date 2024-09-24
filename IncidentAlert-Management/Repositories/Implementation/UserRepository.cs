using IncidentAlert_Management.Data;
using IncidentAlert_Management.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IncidentAlert_Management.Repositories.Implementation
{
    public class UserRepository(DataContext dataContext) : IUserRepository
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task Add(ApplicationUser user)
        {
            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteByUsername(string username)
        {
            var user = await GetByUsername(username);
            if (user != null)
            {
                _dataContext.Users.Remove(user);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(Expression<Func<ApplicationUser, bool>> predicate)
             => await _dataContext.Users.AnyAsync(predicate);

        public async Task<ApplicationUser?> Find(Expression<Func<ApplicationUser, bool>> predicate)
             => await _dataContext.Users.FirstOrDefaultAsync(predicate);

        public async Task<IEnumerable<ApplicationUser>> FindAll(Expression<Func<ApplicationUser, bool>> predicate)
             => await _dataContext.Users.Where(predicate).ToListAsync();


        public async Task<IEnumerable<ApplicationUser>> GetAll()
             => await _dataContext.Users.ToListAsync();

        public async Task<ApplicationUser?> GetById(string id)
            => await Find(e => e.Id == id);

        public async Task<ApplicationUser?> GetByUsername(string username)
            => await Find(e => e.UserName == username);

        public async Task Update(ApplicationUser user)
        {
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
        }
    }
}
