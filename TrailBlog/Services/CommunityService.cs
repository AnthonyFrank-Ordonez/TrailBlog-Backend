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

        public async Task<IEnumerable<CommunityResponseDto>> GetAllCommunitiesAsync()
        {
            return await _context.Communities
                .OrderByDescending(c => c.CreatedAt)
                .Include(c => c.Posts)
                .Include(c => c.User)
                .Select(c => new CommunityResponseDto
                {
                    Id = c.Id,
                    CommunityName = c.Name,
                    Owner = c.User.Username,
                    Posts = c.Posts.Select(p => new PostResponseDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        Author = p.Author,
                        Slug = p.Slug,
                        CreatedAt = p.CreatedAt
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<CommunityResponseDto?> GetCommunityAsync(Guid id)
        {
            var community = await _context.Communities
                .Include(c => c.Posts)
                .Include(c => c.User)
                .Select(c => new CommunityResponseDto
                {
                    Id = c.Id,
                    CommunityName = c.Name,
                    Owner = c.User.Username,
                    Posts = c.Posts.Select(p => new PostResponseDto
                    {
                        Id = p.Id,
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

        public async Task<IEnumerable<CommunityResponseDto>> GetUserCommunitiesAsync(Guid userId)
        {
            var userCommunities = await _context.UserCommunities
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Community)
                .ThenInclude(uc => uc.Posts)
                .ThenInclude(uc => uc.User)
                .Select(uc => new CommunityResponseDto
                {
                    Id = uc.Community.Id,
                    CommunityName = uc.Community.Name,
                    Owner = uc.Community.User.Username,
                    Posts = uc.Community.Posts.Select(p => new PostResponseDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        Author = p.Author,
                        Slug = p.Slug,
                        CreatedAt = p.CreatedAt,
                    })
                    .ToList()
                })
                .ToListAsync();

            return userCommunities;
        }

        public async Task<IEnumerable<UserResponseDto>> GetCommunityMembersAsync(Guid communityId)
        {
            var members = await _context.UserCommunities
                .Where(uc => uc.CommunityId == communityId)
                .Include(uc => uc.User)
                .Select(uc => new UserResponseDto
                {
                    Id = uc.User.Id,
                    Username = uc.User.Username,
                    Email = uc.User.Email,
                })
                .ToListAsync();

            return members;
        }

        public async Task<CommunityResponseDto?> CreateCommunity(CommunityDto community, Guid userId)
        {
            if (community is null)
            {
                return null;
            }

            var newCommunity = new Community();

            newCommunity.Name = community.Name;
            newCommunity.Description = community.Description ?? "";
            newCommunity.OwnerId = userId;
            newCommunity.CreatedAt = DateTime.UtcNow;
            newCommunity.UpdatedAt = DateTime.UtcNow;
            newCommunity.OwnerId = userId;

            _context.Communities.Add(newCommunity);
            await _context.SaveChangesAsync();

            var user = _context.Users.Find(userId);

            return new CommunityResponseDto
            {
                Id = newCommunity.Id,
                CommunityName = newCommunity.Name,
                Owner = user?.Username ?? "Unknown",
                Posts = newCommunity.Posts.Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt
                })
                .ToList()
            };
        }

        public async Task<IEnumerable<CommunityResponseDto>> SearchCommunities(string query)
        {
            var communities = await _context.Communities
                .Where(c => c.Name.Contains(query))
                .OrderByDescending(c => c.CreatedAt)
                .Include(c => c.Posts)
                .Include(c => c.User)
                .Select(c => new CommunityResponseDto
                {
                    Id = c.Id,
                    CommunityName = c.Name,
                    Owner = c.User.Username,
                    Posts = c.Posts.Select(p => new PostResponseDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        Author = p.Author,
                        Slug = p.Slug,
                        CreatedAt = p.CreatedAt
                    })
                    .ToList()
                })
                .ToListAsync();

            return communities;
        }
    }
}
