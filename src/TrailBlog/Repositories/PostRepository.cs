using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<Post> GetPostsDetails()
        {
            return _dbSet
                .Include(p => p.Community)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .AsNoTracking();
        }

        public async Task<Post?> GetPostDetailByIdAsync(Guid id)
        {
            return await GetPostsDetails()
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
