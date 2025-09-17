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
        Task<CommunityResponseDto?> CreateCommunity(CommunityDto community, Guid userId);
        Task<IEnumerable<CommunityResponseDto>> SearchCommunities(string query);
    }
}
        