using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface ILikeRepository : IRepository<Like>
    {
        Task<Like?> GetExistingLikeAsync(Guid userId, Guid postId);

    }
}
