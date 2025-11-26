using System.Linq.Expressions;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface ISavedPostRepository : IRepository<SavedPost>
    {
        IQueryable<SavedPost> GetSavedPostDetail(bool readOnly = true);
        IQueryable<SavedPost> GetSavedPosts(Expression<Func<SavedPost, bool>> predicate, bool isReadOnly = true);
        Task<bool> ExistingSavedPostAsync(Guid userId, Guid postId);

    }
}
