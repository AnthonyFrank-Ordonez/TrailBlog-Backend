using TrailBlog.Entities;

namespace TrailBlog.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsersWithRolesAsync();
        Task<IEnumerable<User>> GetAllAdminUsersAsync();
        Task<IEnumerable<User>> GetAllNonAdminUsersAsync();
        Task<User?> GetUserByIdWithRolesAsync(Guid userId);
        Task<User?> GetUserByUsernameWithRolesAsync(string username);
    }
}
