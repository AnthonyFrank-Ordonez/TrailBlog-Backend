using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _dbSet
                .Include(p => p.Community)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(); 
        }

        public async Task<Post?> GetPostDetailByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Community)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetRecentPostsAsync(int take)
        {
            return await _dbSet
                .Include(p => p.Community)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take) 
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetRecentPostsPagedAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            return await _dbSet
                .Include(p => p.Community)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        }
    }
}
