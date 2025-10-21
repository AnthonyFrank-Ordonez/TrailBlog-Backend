using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface ICommunityService
    {
        Task<PagedResultDto<CommunityResponseDto>> GetCommunitiesPagedAsync(int page = 1, int pageSize = 10);
        Task<PagedResultDto<PostResponseDto>> GetCommunityPostsPagedAsync(Guid id, Guid userId, int page = 1, int pageSize = 10);
        Task<IEnumerable<CommunityResponseDto>> GetUserCommunitiesAsync(Guid userId);
        Task<IEnumerable<UserResponseDto>> GetCommunityMembersAsync(Guid communityId);
        Task<CommunityResponseDto> CreateCommunityAsync(CommunityDto community, Guid userId);
        Task<OperationResultDto> UpdateCommunityAsync(Guid communityId, Guid userId, CommunityDto community, bool isAdmin = false);
        Task<OperationResultDto> DeleteCommunityAsync(Guid communityId, Guid userId, bool isAdmin = false);
        Task<IEnumerable<CommunityResponseDto>> SearchCommunitiesAsync(string query);
        Task<CommunityResponseDto> JoinCommunityAsync(Guid communityId, Guid userId);
        Task<OperationResultDto> LeaveCommunityAsync(Guid communityId, Guid userId);
    }
}
        