using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TrailBlog.Api.Entities;
using TrailBlog.Api.Exceptions;
using TrailBlog.Api.Models;
using TrailBlog.Api.Repositories;

namespace TrailBlog.Api.Services
{
    public class CommunityService(
        ICommunityRepository communityRepository,
        IUserRepository userRepository,
        IPostRepository postRepository,
        IUnitOfWork unitOfWork,
        ILogger<CommunityService> logger,
        IUserCommunityRepository userCommunityRepository) : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository = communityRepository;
        private readonly IUserCommunityRepository _userCommunityRepository = userCommunityRepository;
        private readonly IPostRepository _postRepository = postRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger _logger = logger;

        public async Task<PagedResultDto<CommunityResponseDto>> GetCommunitiesPagedAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _communityRepository.GetCommunityDetails();

            var totalCount = await query.CountAsync();

            var communities = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CommunityResponseDto
                {
                    Id = c.Id,
                    CommunityName = c.Name,
                    Owner = c.User.Username,
                    TotalPosts = c.Posts.Count,
                })
                .ToListAsync();

            return new PagedResultDto<CommunityResponseDto>
            {
                Data = communities,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<PagedResultDto<PostResponseDto>> GetCommunityPostsPagedAsync(Guid id, Guid? userId, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var community = await _communityRepository.GetCommunityDetails()
                .Where(c => c.Id == id)
                .Select(c => new CommunityResponseDto
                {
                    Id = c.Id,
                    CommunityName = c.Name,
                    Owner = c.User.Username,
                })
                .FirstOrDefaultAsync();

            if (community is null)
                throw new NotFoundException($"No community found with the id of {id}");

            var query = _postRepository.GetPostsDetails()
                .Where(p => p.CommunityId == id);

            var totalCount = await query.CountAsync();

            var communityPosts = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    IsOwner = userId.HasValue && p.UserId == userId,
                    CreatedAt = p.CreatedAt,
                    TotalComment = p.Comments.Count,
                    Reactions = p.Reactions
                        .GroupBy(r => r.ReactionId)
                        .Select(g => new PostReactionSummaryDto
                        {
                            ReactionId = g.Key,
                            Count = g.Count()
                        })
                        .ToList(),
                    UserReactionsIds = p.Reactions
                        .Where(r => r.UserId == userId)
                        .Select(r => r.ReactionId)
                        .ToList(),
                })
                .ToListAsync();

            return new PagedResultDto<PostResponseDto>
            {
                Data = communityPosts,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Metadata = new Dictionary<string, object>
                {
                    ["communityId"] = community.Id,
                    ["communityName"] = community.CommunityName
                }
            };
        }

        public async Task<IEnumerable<CommunityResponseDto>> GetUserCommunitiesAsync(Guid userId)
        {
            var userCommunities = await _userCommunityRepository.GetUserCommunitiesAsync(userId)
                .Select(uc => new CommunityResponseDto
                {
                    Id = uc.Community.Id,
                    CommunityName = uc.Community.Name,
                    Description = uc.Community.Description ?? null,
                    Owner = uc.Community.User.Username,
                    IsFavorite = uc.IsFavorite
                })
                .ToListAsync();

            return userCommunities;
        }

        public async Task<IEnumerable<UserResponseDto>> GetCommunityMembersAsync(Guid communityId)
        {
            var communityMembers = await _userCommunityRepository.GetCommunityMembersAsync(communityId)
                .Select(c => new UserResponseDto
                {
                    Id = c.User.Id,
                    Username = c.User.Username,
                    Email = c.User.Email,
                })
                .ToListAsync();

            if (communityMembers is null || !communityMembers.Any())
                throw new NotFoundException($"No community with the if of {communityId}");

            return communityMembers;

        }

        public async Task<CommunityResponseDto> FavoriteCommunityAsync(Guid communityId, Guid userId)
        {
            var userCommunity = await _userCommunityRepository.GetUserCommunityAsync(uc => uc.UserId == userId &&  uc.CommunityId == communityId, isReadOnly: false);

            if (userCommunity is null)
                throw new NotFoundException("User Community not found!");

            userCommunity.IsFavorite = true;

            await _userCommunityRepository.UpdateAsync(userCommunity.UserId, userCommunity.CommunityId, userCommunity);
            await _unitOfWork.SaveChangesAsync();

            return new CommunityResponseDto
            {
                Id = userCommunity.Community.Id,
                CommunityName = userCommunity.Community.Name,
                Description = userCommunity.Community.Description,
                Owner = userCommunity.User.Username,
                IsFavorite = userCommunity.IsFavorite,
            };
        }

        public async Task<CommunityResponseDto> UnfavoriteCommunityAsync(Guid communityId, Guid userId)
        {
            var userCommunity = await _userCommunityRepository.GetUserCommunityAsync(uc => uc.UserId == userId && uc.CommunityId == communityId, isReadOnly: false);

            if (userCommunity is null)
                throw new NotFoundException("User Community not found!");

            userCommunity.IsFavorite = false;

            await _userCommunityRepository.UpdateAsync(userCommunity.UserId, userCommunity.CommunityId, userCommunity);
            await _unitOfWork.SaveChangesAsync();

            return new CommunityResponseDto
            {
                Id = userCommunity.Community.Id,
                CommunityName = userCommunity.Community.Name,
                Description = userCommunity.Community.Description,
                Owner = userCommunity.User.Username,
                IsFavorite = userCommunity.IsFavorite,
            };
        }

        public async Task<CommunityResponseDto> CreateCommunityAsync(CommunityDto community, Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of ${userId}");

            _logger.LogInformation("Creating community for userid: {UserId}", userId);

            if (community is null)
                throw new ApplicationException("Invalid community. Please double check your input");

            var newCommunity = new Community {
                Name = community.Name,
                Description = community.Description ?? "",
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var createdCommunity = await _communityRepository.AddAsync(newCommunity);

            if (createdCommunity is null)
                throw new ApplicationException("An error occured. Failed to create community");

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var userCommunity = new UserCommunity
            {
                UserId = user.Id,
                CommunityId = createdCommunity.Id,
                JoinedDate = DateTime.UtcNow,
            };

            var joinedCommunity = await _userCommunityRepository.AddAsync(userCommunity);

            if (joinedCommunity is null)
                throw new ApplicationException("An error occured. Failed to join the community");

            await _unitOfWork.SaveChangesAsync();


            return new CommunityResponseDto
            {
                Id = createdCommunity.Id,
                CommunityName = createdCommunity.Name,
                Owner = user?.Username ?? "Unknown",
                Posts = createdCommunity.Posts.Select(p => new PostResponseDto
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
            var existingCommunity = await _communityRepository.GetByIdAsync(communityId);

            if (existingCommunity is null)
                throw new NotFoundException($"No community found with the id of {communityId}");

            if (existingCommunity.UserId != userId)
                throw new UnauthorizedException("You are not authorized to update this community.");

            UpdateCommunityFields(existingCommunity, community);

            var updatedCommunity = await _communityRepository.UpdateAsync(existingCommunity.Id, existingCommunity);
            await _unitOfWork.SaveChangesAsync();

            if (updatedCommunity is null)
                throw new ApplicationException("An error occured. Failed to update community");

            return new OperationResultDto
            {
                Success = true,
                Message = "Community updated successfully."
            };

        }

        public async Task<OperationResultDto> DeleteCommunityAsync(Guid communityId, Guid userId, bool isAdmin)
        {
            var existingCommunity = await _communityRepository.GetByIdAsync(communityId);

            if (existingCommunity is null)
                throw new NotFoundException($"No community found with the id of {communityId}");

            if (existingCommunity.UserId != userId && !isAdmin)
                throw new UnauthorizedException("You are not authorized to delete this community.");
                
            await _communityRepository.DeleteAsync(existingCommunity);
            await _unitOfWork.SaveChangesAsync();

            return new OperationResultDto
            {
                Success = true,
                Message = "Community deleted successfully."
            };
        }

        public async Task<IEnumerable<CommunityResponseDto>> SearchCommunitiesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ApplicationException("An error occured. search query cannot be empty");

            var communitiesResult = await _communityRepository.SearchCommunities(query)
                .OrderByDescending(c => c.CreatedAt)
                .Select(cr => new CommunityResponseDto
                {
                    Id = cr.Id,
                    CommunityName = cr.Name,
                    Owner = cr.User.Username,
                })
                .ToListAsync();

            return communitiesResult;

        }

        public async Task<CommunityResponseDto> JoinCommunityAsync(Guid communityId, Guid userId)
        {
            var community = await _communityRepository.GetCommunityDetailsAsync(communityId);
            var user = await _userRepository.GetByIdAsync(userId);

            if (community is null)
                throw new NotFoundException($"No community found with the id of {communityId}");

            if (user is null)
                throw new NotFoundException($"No user found with the id of {userId}");

            var existingMember = await _userCommunityRepository.ExistingMemberAsync(communityId, userId);

            if (existingMember != null)
                throw new ApplicationException($"User: {user.Username} is already a member of this community.");

            var userCommunity = new UserCommunity
            {
                UserId = userId,
                CommunityId = communityId,
                JoinedDate = DateTime.UtcNow,
            };

            var joinedCommunity = await _userCommunityRepository.AddAsync(userCommunity);
            await _unitOfWork.SaveChangesAsync();

            if (joinedCommunity is null)
                throw new ApplicationException("An error occured. Failed to joined the community");

            return new CommunityResponseDto
            {
                Id = community.Id,
                CommunityName = community.Name,
                Description = community.Description ?? null,
                Owner = community.User.Username,
            };
        }

        public async Task<OperationResultDto> LeaveCommunityAsync(Guid communityId, Guid userId)
        {
            var userCommunity = await _userCommunityRepository.ExistingMemberAsync(communityId, userId);

            if (userCommunity is null)
                throw new ApplicationException($"User: {userId} is not a member of this community.");

            await _userCommunityRepository.DeleteAsync(userCommunity);
            await _unitOfWork.SaveChangesAsync();

            return new OperationResultDto
            {
                Success = true,
                Message = "User has successfully left the community"
            };

        }

        private static void UpdateCommunityFields(Community existingCommunity, CommunityDto community)
        {
            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(existingCommunity.Name) &&
                !string.Equals(existingCommunity.Name, community.Name, StringComparison.Ordinal))
            {
                existingCommunity.Name = community.Name;
                hasChanges = true;
            }
            if (!string.IsNullOrWhiteSpace(existingCommunity.Description) &&
                !string.Equals(existingCommunity.Description, community.Description, StringComparison.Ordinal))
            {
                existingCommunity.Description = community.Description;
                hasChanges = true;
            }

            if (hasChanges)
            {
                existingCommunity.UpdatedAt = DateTime.UtcNow;
            }

        }
    }
}
