using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<PublicUserResponseDto>> GetAllUsersAsync();
        Task<PublicUserResponseDto?> GetUserAsync(Guid userId);
        Task<IEnumerable<UserResponseDto>> GetAllUsersWithRolesAsync();   
        Task<UserResponseDto?> GetUserWithRolesAsync(Guid userId);
        Task<IEnumerable<UserResponseDto>> GetAllAdminUsersAsync();
        Task<OperationResultDto> RevokedUserAsync(Guid userId);
        Task<OperationResultDto> DeleteUserAsync(Guid userId);
    }   
}   
