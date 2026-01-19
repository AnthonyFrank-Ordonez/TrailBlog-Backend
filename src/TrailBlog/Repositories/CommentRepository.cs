using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<Comment> GetCommentsDetailsAsync(bool readOnly = true)
        {
            var query = _dbSet
                .Include(c => c.User)
                .Include(c => c.Post);
            
            return readOnly ? query.AsNoTracking() : query;
        }

        public IQueryable<Comment> GetCommentsAsync(Expression<Func<Comment, bool>> predicate, bool readOnly = true)
        {
            var query = GetCommentsDetailsAsync(readOnly)
                .Where(predicate);
            
            return query;
        }

        public IQueryable<Comment> GetDeletedCommentsAsync()
        {
            return GetCommentsDetailsAsync()
                .Where(c => c.IsDeleted);
        }

        public async Task<Comment?> GetExistingCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await GetCommentsDetailsAsync(readOnly: true)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId && !c.IsDeleted);
            
            return comment;
        }

    }
}
