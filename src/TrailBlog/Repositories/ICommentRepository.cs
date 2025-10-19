using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        IQueryable<Comment> GetCommentsDetails();
        IQueryable<Comment> GetDeletedComments();
    }
}
