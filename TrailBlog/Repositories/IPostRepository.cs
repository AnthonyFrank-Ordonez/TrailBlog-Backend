using TrailBlog.Entities;

namespace TrailBlog.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IEnumerable<Post>> GetAllPostsDetailsAsync();
        Task<Post?> GetPostDetailByIdAsync(Guid id);
        Task<IEnumerable<Post>> GetRecentPostsAsync(int take = 10);
        Task<IEnumerable<Post>> GetRecentPostsPagedAsync(int page, int pageSize);
    }
}   