using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto request);
        Task<AuthResultDto> LoginAsync(LoginDto request);
        Task<OperationResultDto> LogoutAsync(Guid Id);
        Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<OperationResultDto> AssignRoleAsync(AssignRoleDto request);

    }
}
