using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserAsync(Guid userId);
        Task<IEnumerable<UserResponseDto>> GetAllUsersWithRolesAsync();   
        Task<UserResponseDto?> GetUserWithRolesAsync(Guid userId);
        Task<IEnumerable<UserResponseDto>> GetAllAdminUsersAsync();
        Task<OperationResultDto> RevokedUserAsync(Guid userId);
    }   
}   
