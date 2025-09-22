using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface ICommunityRepository : IRepository<Community>
    {
        Task<IEnumerable<Community?>> GetAllCommunityWithUserAsync();
        Task<IEnumerable<Community?>> GetAllCommunityWithPostsAsync();
        Task<IEnumerable<Community>> GetAllCommunityWithUserandPostAsync();
        Task<Community?> GetCommunityWithUserandPostAsync(Guid communityId);
        Task<IEnumerable<Community>> GetAllCommunityPostsAsync();
        Task<IEnumerable<Community>> SearchCommunityAsync(string SearchQuery);
    }
}