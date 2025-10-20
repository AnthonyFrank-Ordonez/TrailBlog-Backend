using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface ICommunityRepository : IRepository<Community>
    {
        IQueryable<Community> GetCommunityDetails(bool readOnly = true);
        IQueryable<Community> GetRecentCommunities();
        Task<Community?> GetCommunityDetailsAsync(Guid communityId);
        IQueryable<Community> SearchCommunities(string SearchQuery);
    }
}   