using TrailBlog.Entities;
using TrailBlog.Models;

namespace TrailBlog.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto request);
        Task<AuthResultDto> LoginAsync(LoginDto request);
        Task<bool> AssignRoleAsync(AssignRoleDto request);

    }
}
