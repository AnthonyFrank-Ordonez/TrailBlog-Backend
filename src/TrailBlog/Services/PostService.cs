using TrailBlog.Api.Entities;
using TrailBlog.Api.Models;
using TrailBlog.Api.Repositories;
using TrailBlog.Api.Helpers;
using TrailBlog.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace TrailBlog.Api.Services
{
    public class PostService(
        IPostRepository postRepository, 
        IUserRepository userRepository, 
        ICommunityRepository communityRepository,
        IReactionRepository reactionRepository,
        IRandomPaginationService randomPaginationService,
        IUnitOfWork unitOfWork) : IPostService
    {
        private readonly IPostRepository _postRepository = postRepository;
        private readonly IUserRepository _userrepository = userRepository;
        private readonly ICommunityRepository _communityRepository = communityRepository;
        private readonly IReactionRepository _reactionRepository = reactionRepository;
        private readonly IRandomPaginationService _randomPaginationService = randomPaginationService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<PagedResultDto<PostResponseDto>> GetPostsPagedAsync(Guid userId, int page, int pageSize, string? sessionId = null)
        {
            // Validate
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _postRepository.GetPostsDetails();

            var result = await _randomPaginationService.GetRandomPagedAsync(
                query,
                page,
                pageSize,
                sessionId ?? string.Empty,
                p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt,
                    CommunityName = p.Community.Name,
                    CommunityId = p.CommunityId,
                    TotalComment = p.Comments.Count,
                    TotalReactions = p.Reactions.Count,
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
                        .ToList()
                }
            );

            return result;

            //var totalCount = await query.CountAsync();

            //var posts = await query
            //    .OrderByDescending(p => p.CreatedAt)
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize)
            //    .Select(p => new PostResponseDto
            //    {
            //        Id = p.Id,
            //        Title = p.Title,
            //        Content = p.Content,
            //        Author = p.Author,
            //        Slug = p.Slug,
            //        CreatedAt = p.CreatedAt,
            //        CommunityName = p.Community.Name,
            //        CommunityId = p.CommunityId,
            //        TotalComment = p.Comments.Count,
            //        TotalReactions = p.Reactions.Count,
            //        Reactions = p.Reactions
            //            .GroupBy(r => r.ReactionId)
            //            .Select(g => new PostReactionSummaryDto {
            //                ReactionId = g.Key,
            //                Count = g.Count()
            //            })
            //            .ToList(),
            //        UserReactionsIds = p.Reactions
            //            .Where(r => r.UserId == userId)
            //            .Select(r => r.ReactionId)
            //            .ToList()
            //    })
            //    .ToListAsync();

            //return new PagedResultDto<PostResponseDto>
            //{
            //    Data = posts,
            //    Page = page,
            //    PageSize = pageSize,
            //    TotalCount = totalCount,
            //    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            //};
        }

        public async Task<PostResponseDto?> GetPostAsync(Guid id, Guid userId)
        {
            var post = await _postRepository.GetPostDetailByIdAsync(id);

            if (post is null)
                throw new NotFoundException($"No posts found with the id of {id}");

            return new PostResponseDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Author = post.Author,
                Slug = post.Slug,
                CreatedAt = post.CreatedAt,
                CommunityName = post.Community.Name,
                CommunityId = post.CommunityId,
                TotalComment = post.Comments.Count,
                TotalReactions = post.Reactions.Count,
                Reactions = post.Reactions
                        .GroupBy(r => r.ReactionId)
                        .Select(g => new PostReactionSummaryDto
                        {
                            ReactionId = g.Key,
                            Count = g.Count()
                        })
                        .ToList(),
                UserReactionsIds = post.Reactions
                        .Where(r => r.UserId == userId)
                        .Select(r => r.ReactionId)
                        .ToList(),
                Comments = post.Comments.Select(c => new CommentResponseDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CommentedAt = c.CommentedAt,
                    LastUpdatedAt = c.LastUpdatedAt,
                    IsDeleted = c.IsDeleted,
                })
                .ToList()
            };
        }

        public async Task<PostResponseDto> CreatePostAsync(PostDto post, Guid userId)
        {

            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var community = await _communityRepository.GetByIdAsync(post.CommunityId);

            if (community is null)
                throw new NotFoundException($"Community not found with the id of {post.CommunityId}");

            var newPost = new Post
            {
                Title = post.Title,
                Content = post.Content,
                Author = user.Username,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Slug = post.Title.ToLower().Replace(" ", "-"),
                UserId = user.Id,
                CommunityId = community.Id
            };

            var createdPost = await _postRepository.AddAsync(newPost);
            await _unitOfWork.SaveChangesAsync();

            return new PostResponseDto
            {
                Id = createdPost.Id,
                Title = createdPost.Title,
                Content = createdPost.Content,
                Author = createdPost.Author,
                Slug = createdPost.Slug,
                CreatedAt = createdPost.CreatedAt,
                CommunityId = createdPost.CommunityId,
            };
        }

        public async Task<OperationResultDto> UpdatePostAsync(Guid id, Guid userId, UpdatePostDto post, bool isAdmin)
        {
            var existingPost = await _postRepository.GetByIdAsync(id);

            if (existingPost is null)
                throw new NotFoundException($"No posts found with the id of {id}");

            if (existingPost.UserId != userId)
                throw new UnauthorizedException("You are not authorized to update this post.");

            UpdatePostFields(existingPost, post);

            var updatedPost = await _postRepository.UpdateAsync(existingPost.Id, existingPost);
            await _unitOfWork.SaveChangesAsync();

            if (updatedPost is null)
                throw new ApiException("An error occurred. Post failed to update");

            return OperationResult.Success("Post updated successfully");
        }

        public async Task<OperationResultDto> DeletePostAsync(Guid id, Guid userId, bool isAdmin)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post is null)
                throw new NotFoundException($"No posts found with the id of {id}");

            if (post.UserId != userId && !isAdmin)
                throw new UnauthorizedException("You are not authorized to update this post.");

            await _postRepository.DeleteAsync(post);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Post deleted successfully");
        }
        
        public async Task<PostResponseDto> TogglePostReactionAsync(Guid userId, Guid postId, AddReactionDto reaction)
        {
            var post = await _postRepository.GetPostDetailByIdAsync(postId, isReadOnly: false);
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null) throw new NotFoundException($"User with the id of {userId} not found");
            if (post is null) throw new NotFoundException($"Post with the id of {postId} not found");

            var existingReaction = await _reactionRepository.GetExistingReactionAsync(user.Id, post.Id, reaction.ReactionId);

            if (existingReaction != null)
            {
                await _reactionRepository.DeleteAsync(existingReaction);
            }
            else
            {
                var newReaction = (new Reaction
                {
                    PostId = post.Id,
                    UserId = user.Id,
                    ReactionId = reaction.ReactionId,
                    ReactedAt = DateTime.UtcNow,
                });

                await _reactionRepository.AddAsync(newReaction);
            }

            await _unitOfWork.SaveChangesAsync();

            return CreatePostResponse(post, userId);
        }

        private static void UpdatePostFields(Post existingPost, UpdatePostDto post)
        {
            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(post.Title) && 
                !string.Equals(existingPost.Title, post.Title, StringComparison.Ordinal))
            {
                existingPost.Title = post.Title;
                existingPost.Slug = post.Title.ToLower().Replace(" ", "-");
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(post.Content) &&
                !string.Equals(existingPost.Content, post.Content, StringComparison.Ordinal))
            {
                existingPost.Content = post.Content;
                hasChanges = true;
            }
            if (!string.IsNullOrWhiteSpace(post.Author) && 
                !string.Equals(existingPost.Author, post.Author, StringComparison.Ordinal))
            {
                existingPost.Author = post.Author;
                hasChanges = true;
            }

            if (hasChanges)
            {
                existingPost.UpdatedAt = DateTime.UtcNow;
            }

        }

        private PostResponseDto CreatePostResponse(Post post, Guid userId)
        {
            return new PostResponseDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Author = post.Author,
                Slug = post.Slug,
                CreatedAt = post.CreatedAt,
                CommunityName = post.Community.Name,
                CommunityId = post.CommunityId,
                TotalComment = post.Comments.Count,
                TotalReactions = post.Reactions.Count,
                Reactions = post.Reactions
                        .GroupBy(r => r.ReactionId)
                        .Select(g => new PostReactionSummaryDto
                        {
                            ReactionId = g.Key,
                            Count = g.Count()
                        })
                        .ToList(),
                UserReactionsIds = post.Reactions
                        .Where(r => r.UserId == userId)
                        .Select(r => r.ReactionId)
                        .ToList(),
            };
        }
    }

}
