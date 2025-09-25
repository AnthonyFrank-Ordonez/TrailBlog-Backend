using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Comment>> GetAllDeletedComments()
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Post)
                .Where(c => c.IsDeleted)
                .ToListAsync();
        }

    }
}
