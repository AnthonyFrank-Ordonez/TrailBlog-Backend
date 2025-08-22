using TrailBlog.Entities;
using TrailBlog.Models;

namespace TrailBlog.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto request);
        Task<AuthResultDto> LoginAsync(LoginDto request);
        Task<bool> LogoutAsync(Guid Id);
        Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<bool> AssignRoleAsync(AssignRoleDto request);

    }
}
