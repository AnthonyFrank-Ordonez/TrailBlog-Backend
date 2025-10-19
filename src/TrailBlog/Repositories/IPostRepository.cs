using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        IQueryable<Post> GetPostsDetails();
        Task<Post?> GetPostDetailByIdAsync(Guid id);
    }
}   