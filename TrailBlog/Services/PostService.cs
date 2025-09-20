using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrailBlog.Data;
using TrailBlog.Entities;
using TrailBlog.Helpers;
using TrailBlog.Models;
using TrailBlog.Repositories;

namespace TrailBlog.Services
{
    public class PostService(IPostRepository postRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : IPostService
    {
        private readonly IPostRepository _postRepository = postRepository;
        private readonly IUserRepository _userrepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<PostResponseDto?>> GetPostsAsync()
        {
            var posts = await _postRepository.GetAllPostsDetailsAsync();

            return posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                Author = p.Author,
                Slug = p.Slug,
                CreatedAt = p.CreatedAt,
                Username = p.User.Username,
                CommunityName = p.Community.Name,
                CommunityId = p.CommunityId,
            }).ToList();
        }

        public async Task<PostResponseDto?> GetPostAsync(Guid id)
        {
            var post = await _postRepository.GetPostDetailByIdAsync(id);

            if (post is null) return null;

            return new PostResponseDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Author = post.Author,
                Slug = post.Slug,
                CreatedAt = post.CreatedAt,
                Username = post.User.Username,
                CommunityName = post.Community.Name,
                CommunityId = post.CommunityId,
            };
        }

        public async Task<PostResponseDto?> CreatePostAsync(PostDto post, Guid userId)
        {
            if (post is null)
            {
                return null;
            }

            var newPost = new Post
            {
                Title = post.Title,
                Content = post.Content,
                Author = post.Author,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Slug = post.Title.ToLower().Replace(" ", "-"),
                UserId = userId,
                CommunityId = post.CommunityId
            };

            var createdPost = await _postRepository.AddAsync(newPost);
            await _unitOfWork.SaveChangesAsync();

            var user = await _userrepository.GetByIdAsync(userId);


            return new PostResponseDto
            {
                Id = createdPost.Id,
                Title = createdPost.Title,
                Content = createdPost.Content,
                Author = createdPost.Author,
                Slug = createdPost.Slug,
                CreatedAt = createdPost.CreatedAt,
                Username = user?.Username ?? string.Empty,
                CommunityName = post.CommunityName,
                CommunityId = createdPost.CommunityId,
            };
        }

        public async Task<OperationResultDto> UpdatePostAsync(Guid id, Guid userId, PostDto post, bool isAdmin)
        {
            var existingPost = await _postRepository.GetByIdAsync(id);

            if (existingPost is null) return OperationResult.Failure("Post not found!");

            if (existingPost.UserId != userId && !isAdmin) return OperationResult.Failure("You are not authorized to update this post.");

            UpdatePostFields(existingPost, post);

            var updatedPost = await _postRepository.UpdateAsync(existingPost.Id, existingPost);

            return updatedPost is null
                ? OperationResult.Failure("Failed to update the post.")
                : OperationResult.Success("Post updated successfully"); 
        }

        public async Task<OperationResultDto> DeletePostAsync(Guid id, Guid userId, bool isAdmin)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post is null) return OperationResult.Failure("Post not found!");

            if (post.UserId != userId && !isAdmin) return OperationResult.Success("You are not authorized to delete this post");

            var deletedPost = await _postRepository.DeleteAsync(post.Id);;

            return !deletedPost
                ? OperationResult.Failure("Failed to delete the post")
                : OperationResult.Success("Post deleted successfully");
        }

        public async Task<IEnumerable<PostResponseDto>> GetRecentPostsAsync(int page, int pageSize)
        {
            var posts = await _postRepository.GetRecentPostsPagedAsync(page, pageSize);

            return posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                Author = p.Author,
                Slug = p.Slug,
                CreatedAt = p.CreatedAt,
                Username = p.User.Username,
                CommunityName = p.Community.Name,
                CommunityId = p.CommunityId,
            }).ToList();
        }

        private static void UpdatePostFields(Post existingPost, PostDto post)
        {
            if (!string.IsNullOrWhiteSpace(post.Title))
            {
                existingPost.Title = post.Title;
                existingPost.Slug = post.Title.ToLower().Replace(" ", "-");
            }

            if (!string.IsNullOrWhiteSpace(post.Content)) existingPost.Content = post.Content;
            if (!string.IsNullOrWhiteSpace(post.Author)) existingPost.Author = post.Author;

            existingPost.UpdatedAt = DateTime.UtcNow;
        }
    }

}
