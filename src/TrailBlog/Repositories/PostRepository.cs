using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<Post> GetPostsDetails(bool readOnly = true, PostStatus? statusFilter = null)
        {
            var status = statusFilter ?? PostStatus.Published;

            var query = _dbSet
                .Where(p => p.Status == status)
                .Include(p => p.Community)
                .Include(p => p.Reactions)
                .Include(p => p.SavedPosts)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User);

            return readOnly ? query.AsNoTracking() : query;
        }

        public IQueryable<Post> GetUserPostDraftsAsync(Guid userId, bool isReadOnly = true)
        {
            return GetPostsDetails(readOnly: isReadOnly, statusFilter: PostStatus.Draft)
                .Where(p => p.UserId == userId);
        }

        public IQueryable<Post> GetUserArchivePostsAsync(Guid userId, bool isReadOnly = true)
        {
            return GetPostsDetails(readOnly: isReadOnly, statusFilter: PostStatus.Archived)
                .Where(p => p.UserId == userId);
        }

        public async Task<Post?> GetPostDetailByIdAsync(Guid id, bool isReadOnly = true)
        {
            return await GetPostsDetails(readOnly: isReadOnly)  
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post?> GetPostDetailBySlugAsync(string slug, bool isReadOnly = true)
        {
            return await GetPostsDetails(readOnly: isReadOnly)
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }
    }
}
