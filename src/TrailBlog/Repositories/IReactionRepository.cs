using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IReactionRepository : IRepository<Reaction>
    {
        Task<Reaction?> GetExistingReactionAsync(Guid userId, Guid postId, int reactionId);
    }
}
