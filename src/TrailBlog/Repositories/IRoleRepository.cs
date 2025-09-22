using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetRoleByIdAsync(int roleId);
        Task<Role?> GetRoleByNameAsync(string roleName);
    }
}
