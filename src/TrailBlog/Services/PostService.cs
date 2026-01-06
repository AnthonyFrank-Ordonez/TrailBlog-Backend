using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using TrailBlog.Api.Entities;
using TrailBlog.Api.Exceptions;
using TrailBlog.Api.Extensions;
using TrailBlog.Api.Helpers;
using TrailBlog.Api.Models;
using TrailBlog.Api.Repositories;

namespace TrailBlog.Api.Services
{
    public class PostService(
        IPostRepository postRepository, 
        IUserRepository userRepository, 
        ICommunityRepository communityRepository,
        IUserCommunityRepository userCommunityRepository,
        IReactionRepository reactionRepository,
        IRecentViewedPostRepository recentViewedPostRepository,
        ISavedPostRepository savedPostRepository,
        IUnitOfWork unitOfWork,
        ILogger<PostService> logger,
        IMemoryCache cache,
        IHttpContextAccessor httpContextAccessor) : IPostService
    {
        private readonly IPostRepository _postRepository = postRepository;
        private readonly IUserRepository _userrepository = userRepository;
        private readonly ICommunityRepository _communityRepository = communityRepository;
        private readonly IUserCommunityRepository _userCommunityRepository = userCommunityRepository;
        private readonly IReactionRepository _reactionRepository = reactionRepository;
        private readonly IRecentViewedPostRepository _recentViewedPostRepository = recentViewedPostRepository;
        private readonly ISavedPostRepository _savedPostRepository = savedPostRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<PostService> _logger = logger;
        private readonly IMemoryCache _cache = cache;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;


        public async Task<PagedResultDto<PostResponseDto>> GetPostsPagedAsync(Guid? userId, int page, int pageSize, string? sessionId = null)
        {
            var result = await _postRepository.GetPostsDetails()
                .ToRandomPagedAsync(
                _cache,
                page, 
                pageSize, 
                sessionId,
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
                    IsOwner = userId.HasValue && p.UserId == userId.Value,
                    IsSaved = userId.HasValue && p.SavedPosts.Any(sp => sp.UserId == userId.Value),
                    TotalComment = p.Comments.Count(c => !c.IsDeleted),
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
                },
                _httpContextAccessor
             );

            return result;
        }

        public async Task<PagedResultDto<PostResponseDto>> GetPopularPostsPagedAsync(Guid? userId, int page, int pageSize)
        {
            var result = await _postRepository.GetPostsDetails()
                .ToPagedAsync(
                page,
                pageSize,
                selector: p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt,
                    CommunityName = p.Community.Name,
                    CommunityId = p.CommunityId,
                    IsOwner = userId.HasValue && p.UserId == userId.Value,
                    IsSaved = userId.HasValue && p.SavedPosts.Any(sp => sp.UserId == userId.Value),
                    TotalComment = p.Comments.Count(c => !c.IsDeleted),
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
                },
                orderBy: p => (p.Reactions.Count * 2) + (p.Comments.Count * 3),
                descending: true);

            return result;
        }

        public async Task<PagedResultDto<PostResponseDto>> GetExploredPostsPagedAsync(Guid? userId, int page, int pageSize, string? sessionId = null)
        {
            IEnumerable<Guid> userCommunityIds = [];

            if (userId.HasValue)
            {
                userCommunityIds = await _userCommunityRepository
                    .GetUserCommunitiesAsync(userId.Value)
                    .Select(uc => uc.CommunityId)
                    .ToListAsync();

            }

            var exploredPostQuery = _postRepository.GetPostsDetails()
                .Where(p => !userCommunityIds.Contains(p.CommunityId));

            var result = await exploredPostQuery.ToRandomPagedAsync(
                _cache,
                page,
                pageSize,
                sessionId,
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
                    IsOwner = userId.HasValue && p.UserId == userId.Value,
                    IsSaved = userId.HasValue && p.SavedPosts.Any(sp => sp.UserId == userId.Value),
                    TotalComment = p.Comments.Count(c => !c.IsDeleted),
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
                },
                _httpContextAccessor
            );

            if (result.TotalCount == 0 && userId.HasValue && userCommunityIds.Any())
            {
                // PLAN: If no explored posts are found, fallback to popular posts or non-joined communities

                result.Metadata = new Dictionary<string, object>
                {
                    ["message"] = "You've explored all available posts. There is nothing more to show",
                    ["code"] = "N0_EXPL0RED_POSTS",
                    ["allCommunitiesJoined"] = true,
                };
            }

            return result;
        }

        public async Task<PagedResultDto<PostResponseDto>> GetSavedPostsPagedAsync(Guid userId, int page, int pageSize)
        {
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var result = await _savedPostRepository
                .GetSavedPosts(sp => sp.UserId == userId)
                .Select(sp => sp.Post)
                .ToPagedAsync(
                page,
                pageSize,
                selector: p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt,
                    CommunityName = p.Community.Name,
                    CommunityId = p.CommunityId,
                    IsOwner = p.UserId == userId,
                    IsSaved = true,
                    TotalComment = p.Comments.Count(c => !c.IsDeleted),
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
                },
                orderBy: p => (p.Reactions.Count * 2) + (p.Comments.Count * 3),
                descending: true);

            return result;
        }

        public async Task<PagedResultDto<PostResponseDto>> GetUserPublishedPostsPagedAsync(Guid userId, int page, int pageSize)
        {
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var result = await _postRepository.GetUserPublishedPostsAsync(userId).ToPagedAsync(
                page,
                pageSize,
                selector: p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt,
                    CommunityName = p.Community.Name,
                    CommunityId = p.CommunityId,
                    IsOwner = true,
                    TotalComment = p.Comments.Count(c => !c.IsDeleted),
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
                },
                orderBy: p => p.UpdatedAt,
                descending: true);

            return result;
        }

        public async Task<PagedResultDto<PostResponseDto>> GetUserDraftsPagedAsync(Guid userId, int page, int pageSize)
        {
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var result = await _postRepository.GetUserPostDraftsAsync(userId)
                .ToPagedAsync(
                page,
                pageSize,
                selector: p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt,
                    CommunityName = p.Community.Name,
                    CommunityId = p.CommunityId,
                    IsOwner = p.UserId == userId,
                    TotalComment = 0,
                    TotalReactions = 0,
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
                },
                orderBy: p => p.UpdatedAt,
                descending: true);

            return result;

        }

        public async Task<PagedResultDto<PostResponseDto>> GetUserArchivePostsAsync(Guid userId, int page, int pageSize)
        {
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var result = await _postRepository.GetUserArchivePostsAsync(userId)
                .ToPagedAsync(
                page,
                pageSize,
                selector: p => new PostResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Author = p.Author,
                    Slug = p.Slug,
                    CreatedAt = p.CreatedAt,
                    CommunityName = p.Community.Name,
                    CommunityId = p.CommunityId,
                    IsOwner = p.UserId == userId,
                    IsSaved = p.SavedPosts.Any(sp => sp.UserId == userId),
                    TotalComment = p.Comments.Count(c => !c.IsDeleted),
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
                },
                orderBy: p => (p.Reactions.Count * 2) + (p.Comments.Count * 3),
                descending: true);

            return result;
        }

        public async Task<PostResponseDto?> GetPostAsync(Guid id, Guid userId)
        {
            var post = await _postRepository.GetPostDetailByIdAsync(id);

            if (post is null || post.Status == PostStatus.Archived || post.Status == PostStatus.Draft)
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
                IsOwner = post.UserId == userId,
                TotalComment = post.Comments.Count(c => !c.IsDeleted),
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

        public async Task<PostResponseDto?> GetPostBySlugAsync(string slug, Guid? userId)
        {
            var post = await _postRepository.GetPostDetailBySlugAsync(slug);

            if (post is null || post.Status == PostStatus.Archived || post.Status == PostStatus.Draft)
                throw new NotFoundException($"No posts found with the slug of {slug}");

            if (userId.HasValue)
            {
                await TrackPostViewAsync(userId.Value, post.Id);
            }

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
                IsOwner = userId.HasValue && post.UserId == userId.Value,
                IsSaved = userId.HasValue && post.SavedPosts.Any(sp => sp.UserId == userId.Value),
                TotalComment = post.Comments.Count(c => !c.IsDeleted),
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
                Comments = post.Comments
                    .OrderByDescending(c => c.CommentedAt)
                    .Select(c => new CommentResponseDto
                    {
                        Id = c.Id,
                        Content = c.IsDeleted ? "[This comment has been deleted]" : c.Content,
                        Username = c.IsDeleted ? "Unknown" : c.User.Username,
                        CommentedAt = c.CommentedAt,
                        LastUpdatedAt = c.LastUpdatedAt,
                        IsDeleted = c.IsDeleted,
                        IsOwner = userId.HasValue && c.UserId == userId.Value,
                    })
               .ToList()
            };
        }

        public async Task<IEnumerable<RecentViewedPostDto>> GetRecentlyViewedPostAsync(Guid userId, int count = 10)
        {
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var recentViewedPosts = await _recentViewedPostRepository
                .GetRecentViewedPosts(rvp => rvp.UserId == userId)
                .OrderByDescending(rvp => rvp.ViewedAt)
                .Take(count)
                .Select(rvp => new RecentViewedPostDto
                {
                    PostId = rvp.Post.Id,
                    Title = rvp.Post.Title,
                    Content = rvp.Post.Content,
                    Author = rvp.Post.Author,
                    Slug = rvp.Post.Slug,
                    CreatedAt = rvp.Post.CreatedAt,
                    CommunityName = rvp.Post.Community.Name,
                    TotalComment = rvp.Post.Comments.Count(c => !c.IsDeleted),
                    TotalReactions = rvp.Post.Reactions.Count,
                })
                .ToListAsync();

            return recentViewedPosts;
        }

        public async Task<IEnumerable<PostSearchResultDto>> SearchPostsAsync(string query)
        {
            var postResult = await _postRepository.SearchPosts(query)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostSearchResultDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Author = p.Author,
                    Type = "Post"
                })
                .ToListAsync();

            return postResult;
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
                CommunityId = community.Id,
                Status = PostStatus.Published,
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
                CommunityName = community.Name,
                IsOwner = true,
                IsSaved = false,
                TotalComment = 0,
                TotalReactions = 0,
                Reactions = new List<PostReactionSummaryDto>(),
                UserReactionsIds = new List<int>()
            };
        }

        public async Task<PostResponseDto> CreateDraftAsync(PostDto post, Guid userId)
        {
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var community = await _communityRepository.GetByIdAsync(post.CommunityId);

            if (community is null)
                throw new NotFoundException($"Community not found with the id of {post.CommunityId}");

            var newDraft = new Post
            {
                Title = post.Title,
                Content = post.Content,
                Author = user.Username,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Slug = post.Title.ToLower().Replace(" ", "-"),
                UserId = user.Id,
                CommunityId = community.Id,
                Status = PostStatus.Draft,
            };

            var createdDraft = await _postRepository.AddAsync(newDraft);
            await _unitOfWork.SaveChangesAsync();

            return new PostResponseDto
            {
                Id = createdDraft.Id,
                Title = createdDraft.Title,
                Content = createdDraft.Content,
                Author = createdDraft.Author,
                Slug = createdDraft.Slug,
                CreatedAt = createdDraft.CreatedAt,
                CommunityId = createdDraft.CommunityId,
                CommunityName = community.Name,
                IsOwner = true,
                IsSaved = false,
                TotalComment = 0,
                TotalReactions = 0,
                Reactions = new List<PostReactionSummaryDto>(),
                UserReactionsIds = new List<int>()
            };
        }

        public async Task<PostResponseDto> SavedPostAsync(Guid userId, Guid postId)
        {
            var user = await _userrepository.GetByIdAsync(userId);
            
            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var post = await _postRepository.GetPostDetailByIdAsync(postId);

            if (post is null)
                throw new NotFoundException($"Post not found with the id of {postId}");

            var existingSavedPost = await _savedPostRepository.ExistingSavedPostAsync(userId, postId);

            if (existingSavedPost)
            {
                throw new ApiException("Post is already saved.");
            } else
            {
                var newSavedPost = new SavedPost
                {
                    UserId = userId,
                    PostId = postId,
                    SavedAt = DateTime.UtcNow,
                };

                await _savedPostRepository.AddAsync(newSavedPost);
                await _unitOfWork.SaveChangesAsync();
            }

            return CreatePostResponse(post, userId, isSavedOveride: true);
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

        public async Task<PostResponseDto> UpdateDraftAsync(Guid draftId, Guid userId)
        {  
            var draft = await _postRepository.GetPostDetailByIdAsync(draftId, isReadOnly: false, filterType: PostStatus.Draft);

            if (draft is null)
                throw new NotFoundException($"No drafts found with the id of {draftId}");

            if (draft.UserId != userId)
                throw new UnauthorizedException("You are not authorized to publish this draft.");

            if (draft.Status != PostStatus.Draft)
                throw new ApiException("The specified post is not a draft.");

            draft.Status = PostStatus.Published;
            draft.UpdatedAt = DateTime.UtcNow;

            var publishedPost = await _postRepository.UpdateAsync(draft.Id, draft);
            await _unitOfWork.SaveChangesAsync();

            if (publishedPost is null)
                throw new ApiException("An error occurred. Draft failed to publish");

            return CreatePostResponse(draft, userId, showComments: false);
        }

        public async Task<OperationResultDto> ArchivePostAsync(Guid postId, Guid userId)
        {
            var post = await _postRepository.GetByIdAsync(postId);

            if (post is null)
                throw new NotFoundException($"No posts found with the id of {postId}");

            if (post.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to archive this post.");

            post.Status = PostStatus.Archived;
            post.UpdatedAt = DateTime.UtcNow;

            var acrhivedPost = await _postRepository.UpdateAsync(post.Id, post);
            await _unitOfWork.SaveChangesAsync();

            if (acrhivedPost is null)
                throw new ApiException("An error occurred. Post failed to archive");

            return OperationResult.Success("Post archived successfully");
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

        public async Task<OperationResultDto> DeleteSavedPostAsync(Guid userId, Guid postId)
        {
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var post = await _postRepository.GetByIdAsync(postId);

            if (post is null)
                throw new NotFoundException($"Post not found with the id of {postId}");

            var existingSavedPost = await _savedPostRepository
                .FindOneAsync(sp => sp.UserId == userId && sp.PostId == postId);

            if (existingSavedPost is null)
            {
                throw new ApiException("Invalid operation. Post is not saved.");
            } else
            {
                await _savedPostRepository.DeleteAsync(existingSavedPost);
                await _unitOfWork.SaveChangesAsync();
            }

            return OperationResult.Success("Post unsaved successfully");
        }

        public async Task<OperationResultDto> DeleteAllRecentViewedPostAsync(Guid userId)
        {
            var user = await _userrepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User not found with the id of {userId}");

            var deletedCount = await _recentViewedPostRepository
                .DeleteAllRecentViewsAsync(rvp => rvp.UserId == userId);

            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success($"Successfully deleted, {deletedCount} recent viewed posts");
        }

        public async Task<OperationResultDto> DeleteDraftAsync(Guid draftId, Guid userId, bool isAdmin)
        {
            var draft = await _postRepository.GetByIdAsync(draftId);

            if (draft is null)
                throw new NotFoundException($"No drafts found with the id of {draftId}");

            if (draft.UserId != userId && !isAdmin) 
                throw new UnauthorizedException("You are not authorized to delete this draft.");

            await _postRepository.DeleteAsync(draft);
            await _unitOfWork.SaveChangesAsync();

            return OperationResult.Success("Draft deleted successfully");
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

            return CreatePostResponse(post, userId, showComments: true);
        }


        private async Task TrackPostViewAsync(Guid userId, Guid postId)
        {
            const int MAX_RECENT_VIEWS = 20;
            const int DELETE_COUNT = 10;

            var existingView = await _recentViewedPostRepository.FindOneAsync(rvp => rvp.UserId == userId && rvp.PostId == postId);

            if (existingView != null)
            {
                existingView.ViewedAt = DateTime.UtcNow;
                await _recentViewedPostRepository.UpdateAsync(existingView.Id, existingView);
            }
            else
            {
                var newRecentView = new RecentViewedPost
                {
                    UserId = userId,
                    PostId = postId,
                    ViewedAt = DateTime.UtcNow
                };

                await _recentViewedPostRepository.AddAsync(newRecentView);
            }

            await _unitOfWork.SaveChangesAsync();

            var currentCount = await _recentViewedPostRepository
                .GetRecentViewedPosts(rvp => rvp.UserId == userId)
                .CountAsync();

            if (currentCount > MAX_RECENT_VIEWS)
            {
                await _recentViewedPostRepository
                    .DeleteOldestViewsAsync(rvp => rvp.UserId == userId, DELETE_COUNT);
            }
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

        private PostResponseDto CreatePostResponse(Post post, Guid userId, bool? isSavedOveride = null, bool? showComments = null)
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
                IsOwner = post.UserId == userId,
                IsSaved = isSavedOveride ?? post.SavedPosts.Any(sp => sp.UserId == userId),
                TotalComment = post.Comments.Count(c => !c.IsDeleted),
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
                Comments = showComments == true ? post.Comments
                    .OrderByDescending(c => c.CommentedAt)
                    .Select(c => new CommentResponseDto
                    {
                        Id = c.Id,
                        Content = c.IsDeleted ? "[This comment has been deleted]" : c.Content,
                        Username = c.IsDeleted ? "Unknown" : c.User.Username,
                        CommentedAt = c.CommentedAt,
                        LastUpdatedAt = c.LastUpdatedAt,
                        IsDeleted = c.IsDeleted,
                        IsOwner = c.UserId == userId,
                    })
                   .ToList() : null
            };
        }
    }
}
