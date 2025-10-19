using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        IQueryable<User> GetUserDetails();
        IQueryable<User> GetAdminUsers();
        IQueryable<User> GetNonAdminUsers();
        Task<User?> GetUserByIdWithRolesAsync(Guid userId);
        Task<User?> GetUserByUsernameWithRolesAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}
