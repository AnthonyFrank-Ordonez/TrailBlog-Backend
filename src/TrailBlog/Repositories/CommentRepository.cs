using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<Comment> GetCommentsDetails()
        {
            return _dbSet
                .Include(c => c.User)
                .Include(c => c.Post)
                .AsNoTracking();
            
        }

        public IQueryable<Comment> GetDeletedComments()
        {
            return GetCommentsDetails()
                .Where(c => c.IsDeleted);
        }

    }
}
