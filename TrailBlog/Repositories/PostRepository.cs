using Microsoft.EntityFrameworkCore;
using TrailBlog.Data;
using TrailBlog.Entities;

namespace TrailBlog.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Community)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Post?> GetPostByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Community)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetRecentPostsAsync(int take)
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Community)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetRecentPostsWithPaginateAsync(int page, int pageSize)
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Community)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Skip(pageSize)
                .ToListAsync();

        }
    }
}
