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

        public async Task<OperationResultDto> AddPostLikeAsync(Guid userId, Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null) throw new NotFoundException($"User with the id of {userId} not found");
            if (post is null) throw new NotFoundException($"Post with the id of {postId} not found");

            var existingLike = await _likeRepository.GetExistingLikeAsync(user.Id, post.Id);

            if (existingLike != null) {
                if (existingLike.IsLike)
                {
                    await _likeRepository.DeleteAsync(existingLike);
                    await _unitOfWork.SaveChangesAsync();
                    return OperationResult.Success("Removed Like successfully");
                } else {
                    existingLike.IsLike = true;
                    existingLike.LikeAt = DateTime.UtcNow;
                    await _likeRepository.UpdateAsync(existingLike.Id, existingLike);
                    await _unitOfWork.SaveChangesAsync();
                    return OperationResult.Success("Disliked successfully");
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

            return OperationResult.Success("Added like successfuly");


        }

        public async Task<OperationResultDto> AddPostDislikeAsync(Guid userId, Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
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
                    return OperationResult.Success("Removed dislike successfully");
                }
                else
                {
                    existingLike.IsLike = false;
                    existingLike.LikeAt = DateTime.UtcNow;
                    await _likeRepository.UpdateAsync(existingLike.Id, existingLike);
                    await _unitOfWork.SaveChangesAsync();
                    return OperationResult.Success("Like successfully");
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

            return OperationResult.Success("Successfully dislike the post");
        }


    }
}
