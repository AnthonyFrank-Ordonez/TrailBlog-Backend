using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) {}

        public async Task<IEnumerable<User>> GetAllUsersWithRolesAsync()
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAdminUsersAsync()
        {
            var adminRoleName = new[] { "admin" };

            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur =>
                    ur.Role != null &&
                    ur.Role.Name != null &&
                    adminRoleName.Contains(ur.Role.Name.ToLower()))
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllNonAdminUsersAsync()
        {
            var adminRoleName = new[] { "admin" };

            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => !u.UserRoles.Any(ur =>
                    ur.Role != null &&
                    ur.Role.Name != null &&
                    adminRoleName.Contains(ur.Role.Name.ToLower()))
                )
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdWithRolesAsync(Guid userId)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByUsernameWithRolesAsync(string username)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
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
