using TrailBlog.Entities;
using TrailBlog.Models;

namespace TrailBlog.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto?>> GetUsersAsync();
        Task<UserResponseDto?> GetUserAsync(Guid id);
        Task<UserResponseDto?> RevokedUserAsync(Guid id);
    }
}
