using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) {}

        public IQueryable<User> GetUserDetails(bool readOnly = true)
        {
            var query = _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);

            return readOnly ? query.AsNoTracking() : query;
        }

        public IQueryable<User> GetAdminUsers()
        {

            return GetUserDetails().Where(u => u.UserRoles.Any(ur =>
                    ur.Role != null &&
                    ur.Role.Name != null &&
                    ur.Role.Name.ToLower() == "admin"));
        }

        public IQueryable<User> GetNonAdminUsers()
        {

            return GetUserDetails().Where(u => u.UserRoles.Any(ur =>
                    ur.Role != null &&
                    ur.Role.Name != null &&
                    ur.Role.Name.ToLower() != "admin"));
        }

        public async Task<User?> GetUserByIdWithRolesAsync(Guid userId)
        {
            return await GetUserDetails(readOnly: false)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByUsernameWithRolesAsync(string username)
        {
            return await GetUserDetails(readOnly: false)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

    }
}
