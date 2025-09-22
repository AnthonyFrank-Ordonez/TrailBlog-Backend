using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }
    }
}
