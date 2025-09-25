using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        public LikeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Like?> GetExistingLikeAsync(Guid userId, Guid postId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
        }
        
    }
}
