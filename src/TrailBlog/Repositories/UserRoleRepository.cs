using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ApplicationDbContext context) : base(context) { }

        public async Task<UserRole?> RoleExistAsync(Guid userId, int roleId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }
    }
}
