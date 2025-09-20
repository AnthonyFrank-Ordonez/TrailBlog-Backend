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
                    .ThenInclude(c => c.Posts)
                .Include(uc => uc.Community)
                    .ThenInclude(c => c.User)
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

        public async Task<IEnumerable<CommunityResponseDto>> GetAllCommunityPostsAsync()
        {
            var communityBlogs = await _context.Communities
                .Include(c => c.Posts.OrderByDescending(p => p.CreatedAt).Take(5))
                .Select(c => new CommunityResponseDto
                {
                    CommunityName = c.Name,
                    Id = c.Id,
                    Posts = c.Posts.Select(p => new PostResponseDto
                    {
                        Title = p.Title,
                        Content = p.Content,
                        Author = p.Author,
                        Slug = p.Slug,
                        CreatedAt = p.CreatedAt,
                    }).ToList()
                })
                .ToListAsync();

            return communityBlogs;

        }

        public async Task<CommunityResponseDto?> CreateCommunityAsync(CommunityDto community, Guid userId)
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

        public async Task<OperationResultDto> UpdateCommunityAsync(Guid communityId, Guid userId, CommunityDto community, bool isAdmin)
        {
            var existingCommunity = await _context.Communities.FindAsync(communityId);

            if (existingCommunity is null)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Community Not Found!"
                };
            }

            if (existingCommunity.OwnerId != userId || !isAdmin)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "You are not authorized to update this community."
                };
            }

            existingCommunity.Name = string.IsNullOrEmpty(community.Name) ? existingCommunity.Name : community.Name;
            existingCommunity.Description = string.IsNullOrEmpty(community.Name) ? existingCommunity.Description : community.Description;
            existingCommunity.UpdatedAt = DateTime.UtcNow;

            _context.Communities.Update(existingCommunity);
            await _context.SaveChangesAsync();

            return new OperationResultDto
            {
                Success = true,
                Message = "Community updated successfully."
            };

        }

        public async Task<OperationResultDto> DeleteCommunityAsync(Guid communityId, Guid userId, bool isAdmin)
        {
            var existingCommunity = await _context.Communities.FindAsync(communityId);

            if (existingCommunity is null)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Community Not Found!"
                };
            }

            if (existingCommunity.OwnerId != userId || !isAdmin)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "You are not authorized to delete this community."
                };
            }

            _context.Communities.Remove(existingCommunity);
            await _context.SaveChangesAsync();

            return new OperationResultDto
            {
                Success = true,
                Message = "Community deleted successfully."
            };
        }

        public async Task<IEnumerable<CommunityResponseDto>> SearchCommunitiesAsync(string query)
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

        public async Task<OperationResultDto> JoinCommunityAsync(Guid communityId, Guid userId)
        {
            var community = await _context.Communities.FindAsync(communityId);
            var user = await _context.Users.FindAsync(userId);

            if (community is null)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "Community Not Found!"
                };
            }

            if (user is null)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "User Not Found!"
                };
            }

            var existingMember = await _context.UserCommunities
                .FirstOrDefaultAsync(uc => uc.Community.Id == communityId && uc.UserId == userId);

            if (existingMember is null)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "User is already a member of this community."
                };
            }

            var userCommunity = new UserCommunity
            {
                UserId = userId,
                CommunityId = communityId,
                JoinedDate = DateTime.UtcNow,
            };

            _context.UserCommunities.Add(userCommunity);
            await _context.SaveChangesAsync();

            return new OperationResultDto
            {
                Success = true,
                Message = "User has successfully joined the community."
            };
        }

        public async Task<OperationResultDto> LeaveCommunityAsync(Guid communityId, Guid userId)
        {
            var userCommunity = await _context.UserCommunities
                .FirstOrDefaultAsync(uc => uc.CommunityId == communityId && uc.UserId == userId);

            if (userCommunity is null)
            {
                return new OperationResultDto
                {
                    Success = false,
                    Message = "User is not a member of this community."
                };
            }

            _context.UserCommunities.Remove(userCommunity);
            await _context.SaveChangesAsync();

            return new OperationResultDto
            {
                Success = true,
                Message = "User has successfully left the community"
            };

        }
    }
}
