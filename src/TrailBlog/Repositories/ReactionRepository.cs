using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class ReactionRepository : Repository<Reaction>, IReactionRepository
    {
        public ReactionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Reaction?> GetExistingReactionAsync(Guid userId, Guid postId, int reactionId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => 
                    r.UserId == userId && 
                    r.PostId == postId && 
                    r.ReactionId == reactionId
                );
        }
    }

}
