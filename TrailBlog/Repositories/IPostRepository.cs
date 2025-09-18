using TrailBlog.Entities;

namespace TrailBlog.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<IEnumerable<Post>> GetRecentPostsAsync(int take = 10);
        Task<IEnumerable<Post>> GetRecentPostsWithPaginateAsync(int page, int pageSize);
    }
}