using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        IQueryable<Post> GetPostsDetails(bool readOnly = true, PostStatus? statusFilter = null);
        IQueryable<Post> GetUserPostDraftsAsync(Guid userId, bool isReadOnly = true);
        IQueryable<Post> GetUserArchivePostsAsync(Guid userId, bool isReadOnly = true);
        Task<Post?> GetPostDetailByIdAsync(Guid id, bool isReadOnly = true, PostStatus? filterType = null);
        Task<Post?> GetPostDetailBySlugAsync(string slug, bool isReadOnly = true);
    }
}   