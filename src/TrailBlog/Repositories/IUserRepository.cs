using TrailBlog.Api.Entities;
using TrailBlog.Api.Models;

namespace TrailBlog.Api.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsersWithRolesAsync();
        Task<IEnumerable<User>> GetAllAdminUsersAsync();
        Task<IEnumerable<User>> GetAllNonAdminUsersAsync();
        Task<User?> GetUserByIdWithRolesAsync(Guid userId);
        Task<User?> GetUserByUsernameWithRolesAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}
