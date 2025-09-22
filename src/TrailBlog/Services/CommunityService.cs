using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Data;
using TrailBlog.Api.Models;
using TrailBlog.Api.Entities;
using TrailBlog.Api.Repositories;
using TrailBlog.Api.Exceptions;

namespace TrailBlog.Api.Services
{
    public class CommunityService(
        ICommunityRepository communityRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IUserCommunityRepository userCommunityRepository) : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository = communityRepository;
        private readonly IUserCommunityRepository _userCommunityRepository = userCommunityRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        public async Task<IEnumerable<CommunityResponseDto>> GetAllCommunitiesAsync()
        {
            var communities = await _communityRepository.GetAllCommunityWithUserandPostAsync();

            if (communities is null || !communities.Any())
                throw new NotFoundException("No communities found");

            return communities.Select(c => new CommunityResponseDto
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
            }).ToList();

        }

        public async Task<CommunityResponseDto?> GetCommunityAsync(Guid id)
        {
            var community = await _communityRepository.GetCommunityWithUserandPostAsync(id);

            if (community is null)
                throw new NotFoundException($"No community found with the id of {id}");

            return new CommunityResponseDto
            {
                Id = community.Id,
                CommunityName = community.Name,
                Owner = community.User.Username,
                Posts = community.Posts.Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt
                }).ToList()
            };

        }

        public async Task<IEnumerable<CommunityResponseDto>> GetUserCommunitiesAsync(Guid userId)
        {
            var userCommunities = await _userCommunityRepository.GetUserCommunitiesAsync(userId);

            if (userCommunities is null || !userCommunities.Any())
                throw new NotFoundException("No user communities found");

            return userCommunities.Select(uc => new CommunityResponseDto
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
                }).ToList()
            }).ToList();
        }

        public async Task<IEnumerable<UserResponseDto>> GetCommunityMembersAsync(Guid communityId)
        {
            var community = await _userCommunityRepository.GetCommunityMemberAsync(communityId);

            if (community is null || !community.Any())
                throw new NotFoundException($"No community with the if of {communityId}");

            return community.Select(c => new UserResponseDto
            {
                Id = c.User.Id,
                Username = c.User.Username,
                Email = c.User.Email,
            }).ToList();

        }

        public async Task<IEnumerable<CommunityResponseDto>> GetAllCommunityPostsAsync()
        {
            var communityPosts = await _communityRepository.GetAllCommunityPostsAsync();

            if (communityPosts is null || !communityPosts.Any())
                throw new NotFoundException("No comunities found");

            return communityPosts.Select(c => new CommunityResponseDto
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
            });

        }

        public async Task<CommunityResponseDto> CreateCommunityAsync(CommunityDto community, Guid userId)
        {
            if (community is null)
                throw new ApplicationException("Invalid community. Please double check your input");
            

            var newCommunity = new Community {
                Name = community.Name,
                Description = community.Description ?? "",
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var createdCommunity = await _communityRepository.AddAsync(newCommunity);
            await _unitOfWork.SaveChangesAsync();

            if (createdCommunity is null)
                throw new ApplicationException("An error occured. Failed to create community");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

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

            if (existingCommunity.OwnerId != userId)
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

            if (existingCommunity.OwnerId != userId || !isAdmin)
                throw new UnauthorizedException("You are not authorized to delete this community.");

            var result = await _communityRepository.DeleteAsync(existingCommunity.Id);
            await _unitOfWork.SaveChangesAsync();

            if (!result)
                throw new ApplicationException("An error occured. Failed to delete the post");

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

            var communitiesResult = await _communityRepository.SearchCommunityAsync(query);

            if (communitiesResult is null || !communitiesResult.Any())
                throw new NotFoundException("No communities found");

            return communitiesResult.Select(cr => new CommunityResponseDto
            {
                Id = cr.Id,
                CommunityName = cr.Name,
                Owner = cr.User.Username,
                Posts = cr.Posts.Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt
                })
                    .ToList()
            }).ToList();

        }

        public async Task<OperationResultDto> JoinCommunityAsync(Guid communityId, Guid userId)
        {
            var community = await _communityRepository.GetByIdAsync(communityId);
            var user = await _userRepository.GetByIdAsync(userId);

            if (community is null)
                throw new NotFoundException($"No community found with the id of {communityId}");

            if (user is null)
                throw new NotFoundException($"No user found with the id of {userId}");

            var existingMember = await _userCommunityRepository.ExistingMemberAsync(communityId, userId);

            if (existingMember != null)
                throw new ApplicationException("User is already a member of this community.");

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

            return new OperationResultDto
            {
                Success = true,
                Message = "User has successfully joined the community."
            };
        }

        public async Task<OperationResultDto> LeaveCommunityAsync(Guid communityId, Guid userId)
        {
            var userCommunity = await _userCommunityRepository.ExistingMemberAsync(communityId, userId);

            if (userCommunity is null)
                throw new ApplicationException("User is not a member of this community.");

            var result = await _userCommunityRepository.DeleteAsync(communityId);
            await _unitOfWork.SaveChangesAsync();

            if (!result)
                throw new ApplicationException("An error occured. Failed to leave the community");

            return new OperationResultDto
            {
                Success = true,
                Message = "User has successfully left the community"
            };

        }

        private static void UpdateCommunityFields(Community existingCommunity, CommunityDto community)
        {
            if (!string.IsNullOrWhiteSpace(existingCommunity.Name)) existingCommunity.Name = community.Name;
            if (!string.IsNullOrWhiteSpace(existingCommunity.Description)) existingCommunity.Description = community.Description;

            existingCommunity.UpdatedAt = DateTime.UtcNow;

        }
    }
}
