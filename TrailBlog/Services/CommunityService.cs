using Microsoft.EntityFrameworkCore;
using TrailBlog.Data;
using TrailBlog.Entities;
using TrailBlog.Models;

namespace TrailBlog.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ApplicationDbContext _context;
        
        public CommunityService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Community>> GetAllCommunitiesAsync()
        {
            return await _context.Communities
                .OrderByDescending(c => c.CreatedAt)
                .Include(c => c.Posts)
                .Select(c => new Community
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    Posts = c.Posts.OrderByDescending(p => p.CreatedAt).ToList()
                })
                .ToListAsync();
        }

        public async Task<CommunityBlogsResponseDto?> GetCommunity(Guid id)
        {
            var community = await _context.Communities
                .Include(c => c.Posts)
                .Select(c => new CommunityBlogsResponseDto
                {
                    Id = c.Id,
                    CommunityName = c.Name,
                    Posts = c.Posts.Select(p => new PostResponseDto
                    {
                        Title = p.Title,
                        Content = p.Content,
                        Author = p.Author,
                        Slug = p.Slug,
                        CreatedAt = p.CreatedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync(c => c.Id == id);

            return community;
        }
    }
}
