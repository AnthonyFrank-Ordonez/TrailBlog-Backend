using System.Linq.Expressions;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public interface IRecentViewedPostRepository : IRepository<RecentViewedPost>
    {
        IQueryable<RecentViewedPost> GetRecentViewedPostDetail(bool isReadOnly = true);
        IQueryable<RecentViewedPost> GetRecentViewedPosts(Expression<Func<RecentViewedPost, bool>> predicate, bool isReadOnly = true);
        Task<int> DeleteOldestViewsAsync(Expression<Func<RecentViewedPost, bool>> predicate, int count = 10);
        Task<int> DeleteAllRecentViewsAsync(Expression<Func<RecentViewedPost, bool>> predicate);
    }
}
