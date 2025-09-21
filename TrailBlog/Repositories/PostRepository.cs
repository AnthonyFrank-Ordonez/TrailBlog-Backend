using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Post>> GetAllPostsDetailsAsync()
        {
            return await _dbSet
                .Include(p => p.User)   
                .Include(p => p.Community)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(); 
        }

        public async Task<Post?> GetPostDetailByIdAsync(Guid id)
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

        public async Task<IEnumerable<Post>> GetRecentPostsPagedAsync(int page, int pageSize)
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Community)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        }
    }
}
