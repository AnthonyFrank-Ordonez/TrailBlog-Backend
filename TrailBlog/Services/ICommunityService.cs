using TrailBlog.Entities;
using TrailBlog.Models;

namespace TrailBlog.Services
{
    public interface ICommunityService
    {
        Task<IEnumerable<Community>> GetAllCommunitiesAsync();
        Task<CommunityBlogsResponseDto?> GetCommunity(Guid id);
    }
}
