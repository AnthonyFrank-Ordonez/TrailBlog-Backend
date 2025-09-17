using TrailBlog.Entities;
using TrailBlog.Models;

namespace TrailBlog.Services
{
    public interface ICommunityService
    {
        Task<IEnumerable<CommunityResponseDto>> GetAllCommunitiesAsync();
        Task<CommunityResponseDto?> GetCommunityAsync(Guid id);
        Task<IEnumerable<CommunityResponseDto>> GetUserCommunitiesAsync(Guid userId);
        Task<IEnumerable<UserResponseDto>> GetCommunityMembersAsync(Guid communityId); 
        Task<CommunityResponseDto?> CreateCommunityAsync(CommunityDto community, Guid userId);
        Task<OperationResultDto> UpdateCommunityAsync(Guid communityId, Guid userId, CommunityDto community, bool isAdmin = false);
        Task<OperationResultDto> DeleteCommunityAsync(Guid communityId, Guid userId, bool isAdmin = false);
        Task<IEnumerable<CommunityResponseDto>> SearchCommunitiesAsync(string query);
        Task<OperationResultDto> JoinCommunityAsync(Guid communityId, Guid userId);
        Task<OperationResultDto> LeaveCommunityAsync(Guid communityId, Guid userId);
    }
}
        