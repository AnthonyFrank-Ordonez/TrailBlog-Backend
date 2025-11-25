using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class SavedPostRepository : Repository<SavedPost>, ISavedPostRepository
    {
        public SavedPostRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<SavedPost> GetSavedPostDetail(bool readOnly = true)
        {
            var query = _dbSet
                .Include(sp => sp.Post)
                    .ThenInclude(p => p.Community)
                .Include(sp => sp.Post)
                    .ThenInclude(p => p.Reactions)
                .Include(sp => sp.Post)
                    .ThenInclude(p => p.Comments)
                        .ThenInclude(c => c.User)
                .Include(sp => sp.User);

            return readOnly ? query.AsNoTracking() : query;
        }

        public IQueryable<SavedPost> GetSavedPosts(Expression<Func<SavedPost, bool>> predicate, bool isReadOnly = true)
        {
            return GetSavedPostDetail(readOnly: isReadOnly)
                .Where(predicate);
        }
    }
}
