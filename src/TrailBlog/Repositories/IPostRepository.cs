using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<Post?> GetPostDetailByIdAsync(Guid id);
        Task<IEnumerable<Post>> GetRecentPostsAsync(int take = 10);
        Task<IEnumerable<Post>> GetRecentPostsPagedAsync(int page = 1, int pageSize = 10);
    }
}   