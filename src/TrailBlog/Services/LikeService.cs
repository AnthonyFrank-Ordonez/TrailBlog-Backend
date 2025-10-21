using TrailBlog.Api.Entities;
using TrailBlog.Api.Exceptions;
using TrailBlog.Api.Helpers;
using TrailBlog.Api.Models;
using TrailBlog.Api.Repositories;

namespace TrailBlog.Api.Services
{
    public class LikeService(
        ILikeRepository likeRepository, 
        IPostRepository postRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork) : ILikeService
    {
        private readonly ILikeRepository _likeRepository = likeRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPostRepository _postRepository = postRepository;

        public async Task<PostResponseDto> AddPostLikeAsync(Guid userId, Guid postId)
        {
            var post = await _postRepository.GetPostDetailByIdAsync(postId, isReadOnly: false);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null) throw new NotFoundException($"User with the id of {userId} not found");
            if (post is null) throw new NotFoundException($"Post with the id of {postId} not found");

            var existingLike = await _likeRepository.GetExistingLikeAsync(user.Id, post.Id);

            if (existingLike != null) {
                if (existingLike.IsLike)
                {
                    await _likeRepository.DeleteAsync(existingLike);
                    await _unitOfWork.SaveChangesAsync();

                    return CreatePostResponse(post, userId);

                } else {
                    existingLike.IsLike = true;
                    existingLike.LikeAt = DateTime.UtcNow;
                    await _likeRepository.UpdateAsync(existingLike.Id, existingLike);
                    await _unitOfWork.SaveChangesAsync();

                    return CreatePostResponse(post, userId);
                }
            };

            var newLike = new Like
            {
                UserId = user.Id,
                PostId = post.Id,
                IsLike = true,
                LikeAt = DateTime.UtcNow,
            };

            var result = await _likeRepository.AddAsync(newLike);   
            await _unitOfWork.SaveChangesAsync();

            if (result is null) throw new ApplicationException("An error occured. Unable to add like");

            return CreatePostResponse(post, userId);
        }

        public async Task<PostResponseDto> AddPostDislikeAsync(Guid userId, Guid postId)
        {
            var post = await _postRepository.GetPostDetailByIdAsync(postId, isReadOnly: false);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null) throw new NotFoundException($"User with the id of {userId} not found");
            if (post is null) throw new NotFoundException($"Post with the id of {postId} not found");

            var existingLike = await _likeRepository.GetExistingLikeAsync(user.Id, post.Id);

            if (existingLike != null)
            {
                if (!existingLike.IsLike)
                {
                    await _likeRepository.DeleteAsync(existingLike);
                    await _unitOfWork.SaveChangesAsync();
                    return CreatePostResponse(post, userId);
                }
                else
                {
                    existingLike.IsLike = false;
                    existingLike.LikeAt = DateTime.UtcNow;
                    await _likeRepository.UpdateAsync(existingLike.Id, existingLike);
                    await _unitOfWork.SaveChangesAsync();
                    return CreatePostResponse(post, userId);
                }
            };

            var newLike = new Like
            {
                UserId = user.Id,
                PostId = post.Id,
                IsLike = false,
                LikeAt = DateTime.UtcNow,
            };

            var result = await _likeRepository.AddAsync(newLike);
            await _unitOfWork.SaveChangesAsync();

            return CreatePostResponse(post, userId);
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
                TotalLike = post.Likes.Count,
                TotalComment = post.Comments.Count,
                IsLiked = post.Likes.Any(l => l.UserId == userId && l.IsLike == true),
                IsDisliked = post.Likes.Any(l => l.UserId == userId && l.IsLike == false),
            };
        }

    }
}
