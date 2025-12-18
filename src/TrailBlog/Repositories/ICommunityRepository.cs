using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface ICommunityRepository : IRepository<Community>
    {
        IQueryable<Community> GetCommunityDetails(bool readOnly = true);
        IQueryable<Community> GetRecentCommunities();
        IQueryable<Community> SearchCommunity(string searchQuery);
        IQueryable<Community> SearchCommunities(string SearchQuery);
        Task<Community?> GetCommunityDetailsAsync(Guid communityId);
    }
}   