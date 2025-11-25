using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class SavedPostRepository : Repository<SavedPost>, ISavedPostRepository
    {
        public SavedPostRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<SavedPost> GetSavedPostDetail(bool isReadOnly = true)
        {
            var query = _dbSet
                .Include(sp => sp.Post)
                .Include(sp => sp.User);

            return isReadOnly ? query.AsNoTracking() : query;
        }

        public IQueryable<SavedPost> GetSavedPosts(Expression<Func<SavedPost, bool>> predicate, bool isReadOnly = true)
        {
            return GetSavedPostDetail(isReadOnly)
                .Where(predicate);
        }

   
    }
}
