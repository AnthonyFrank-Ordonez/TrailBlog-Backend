using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task<UserRole?> RoleExistAsync(Guid userId, int roleId);
    }
}
