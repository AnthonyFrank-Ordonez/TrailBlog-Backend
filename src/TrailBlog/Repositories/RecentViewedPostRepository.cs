using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class RecentViewedPostRepository : Repository<RecentViewedPost>, IRecentViewedPostRepository
    {
        public RecentViewedPostRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<RecentViewedPost> GetRecentViewedPostDetail(bool isReadOnly = true)
        {
            var query = _dbSet
                .Include(rvp => rvp.Post)
                    .ThenInclude(p => p.Community)
                .Include(rvp => rvp.User);

            return isReadOnly ? query.AsNoTracking() : query;
        }

        public IQueryable<RecentViewedPost> GetRecentViewedPosts(Expression<Func<RecentViewedPost, bool>> predicate, bool isReadOnly = true)
        {
            return GetRecentViewedPostDetail(isReadOnly)
                .Where(predicate);
        }

        public async Task<int> DeleteOldestViewsAsync(Expression<Func<RecentViewedPost, bool>> predicate, int count = 10)
        {
            var idsToDelete = await _dbSet
                .Where(predicate)
                .OrderBy(rvp => rvp.ViewedAt)
                .Take(count)
                .Select(rvp => rvp.Id)
                .ToListAsync();

            return await _dbSet
                .Where(rvp => idsToDelete.Contains(rvp.Id))
                .ExecuteDeleteAsync();

        }

        public async Task<int> DeleteAllRecentViewsAsync(Expression<Func<RecentViewedPost, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .ExecuteDeleteAsync();
        }
    }
}
