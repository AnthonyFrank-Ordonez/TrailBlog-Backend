using System.Linq.Expressions;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        IQueryable<Comment> GetCommentsDetailsAsync(bool readOnly = true);
        IQueryable<Comment> GetCommentsAsync(Expression<Func<Comment, bool>> predicate, bool readOnly = true);
        IQueryable<Comment> GetDeletedCommentsAsync();
        Task<Comment?> GetExistingCommentAsync(Guid commentId, Guid userId);
    }
}
