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

        public IQueryable<Post> GetUserPublishedPostsAsync(Guid userId)
        {
            return GetPostsDetails(readOnly: true)
                .Where(p => p.UserId == userId);
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

        public IQueryable<Post> SearchPosts(string searchQuery, bool isReadOnly = true)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return Enumerable.Empty<Post>().AsQueryable();

            searchQuery = searchQuery.Replace("[", "[[]")
                         .Replace("%", "[%]")
                         .Replace("_", "[_]");


            return GetPostsDetails(readOnly: isReadOnly, statusFilter: PostStatus.Published)
                .Where(p => EF.Functions.ILike(p.Title, $"%{searchQuery}%") ||
                            EF.Functions.ILike(p.Content, $"%{searchQuery}%"));
        }

        public async Task<Post?> GetPostDetailByIdAsync(Guid id, bool isReadOnly = true, PostStatus? filterType = null)
        {
            return await GetPostsDetails(readOnly: isReadOnly, statusFilter: filterType)  
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post?> GetPostDetailBySlugAsync(string slug, bool isReadOnly = true)
        {
            return await GetPostsDetails(readOnly: isReadOnly)
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }
    }
}
