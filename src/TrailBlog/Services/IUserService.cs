using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface IUserService
    {
        Task<PagedResultDto<PublicUserResponseDto>> GetUsersPagedAsync(int page = 1, int pageSize = 10);
        Task<PagedResultDto<UserResponseDto>> GetUsersWithRolesPagedAsync(int page = 1, int pageSize = 10);
        Task<PublicUserResponseDto?> GetUserAsync(Guid userId);
        Task<UserResponseDto?> GetUserWithRolesAsync(Guid userId);
        Task<IEnumerable<UserResponseDto>> GetAllAdminUsersAsync();
        Task<OperationResultDto> RevokedUserAsync(Guid userId);
        Task<OperationResultDto> DeleteUserAsync(Guid userId);
    }   
}   
