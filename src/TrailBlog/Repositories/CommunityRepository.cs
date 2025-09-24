using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class CommunityRepository : Repository<Community>, ICommunityRepository
    {
        public CommunityRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Community?>> GetAllCommunityWithUserAsync()
        {
            return await _dbSet
                .OrderByDescending(c => c.CreatedAt)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Community?>> GetAllCommunityWithPostsAsync()
        {
            return await _dbSet
                .OrderByDescending(c => c.CreatedAt)
                .Include(c => c.Posts)
                .ToListAsync();
        }

        public async Task<IEnumerable<Community>> GetAllCommunityWithUserandPostAsync()
        {
            return await _dbSet
                .OrderByDescending(c => c.CreatedAt)
                .Include(c => c.User)
                .Include(c => c.Posts)
                .ToListAsync();

        }

        public async Task<Community?> GetCommunityWithUserandPostAsync(Guid communityId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Posts)
                .FirstOrDefaultAsync(c => c.Id == communityId);

        }

        public async Task<IEnumerable<Community>> GetAllCommunityPostsAsync()
        {
            return await _dbSet
                .Include(c => c.Posts.OrderByDescending(p => p.CreatedAt).Take(5))
                .ToListAsync();
        }

        public async Task<IEnumerable<Community>> SearchCommunityAsync(string searchQuery)
        {
            return await _dbSet
                .Where(c => c.Name.Contains(searchQuery))
                .OrderByDescending(c => c.CreatedAt)
                .Include(c => c.Posts)
                .Include(c => c.User)
                .ToListAsync();
        }


    }
}
