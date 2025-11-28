using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Entities;
using TrailBlog.Api.Exceptions;
using TrailBlog.Api.Helpers;
using TrailBlog.Api.Models;
using TrailBlog.Api.Repositories;

namespace TrailBlog.Api.Services
{
    public class CommentService(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork) : ICommentService
    {
        private readonly ICommentRepository _commentRepository = commentRepository;
        private readonly IPostRepository _postRepository = postRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<CommentResponseDto>> GetDeletedComments()
        {
            var deletedComments = await _commentRepository
                .GetDeletedComments()
                .Select(dc => new CommentResponseDto
                {
                    Id = dc.Id,
                    UserId = dc.User.Id,
                    PostId = dc.Post.Id,
                    Content = dc.Content,
                    Username = dc.User.Username,
                    LastUpdatedAt = dc.LastUpdatedAt,
                    CommentedAt = dc.CommentedAt,
                    IsDeleted = dc.IsDeleted
                })
                .ToListAsync();

            return deletedComments;
        }

        public async Task<CommentResponseDto> AddCommentAsync(Guid userId, CommentDto comment)
        {
            var post = await _postRepository.GetByIdAsync(comment.PostId);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User with the id of {userId} not found");

            if (post is null)
                throw new NotFoundException($"Post with the id of {comment.PostId} not found");

            var newComment = new Comment
            {
                Content = comment.Content,
                CommentedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                UserId = user.Id,
                PostId = post.Id,
            };

            await _commentRepository.AddAsync(newComment);
            await _unitOfWork.SaveChangesAsync();

            return new CommentResponseDto
            {
                Id = newComment.Id,
                UserId = user.Id,
                PostId = post.Id,
                Username = user.Username,
                Content = newComment.Content,
                CommentedAt = newComment.CommentedAt,
                LastUpdatedAt = newComment.LastUpdatedAt,
                IsDeleted = newComment.IsDeleted,
            };

        }

        public async Task<CommentResponseDto> EditCommentAsync(Guid commentId, Guid userId, UpdateCommentDto comment)
        {
            var existingComment = await _commentRepository.GetByIdAsync(commentId);
            var user = await _userRepository.GetByIdAsync(userId);  

            if (user is null)
                throw new NotFoundException($"User with the id of {userId} not found");

            if (existingComment is null)
                throw new NotFoundException($"Comment with the id of {commentId} not found");

            if (existingComment.UserId != userId)
                throw new UnauthorizedException("You are unauthorize to edit this comment");

            UpdateCommentField(existingComment, comment);

            var updatedComment = await _commentRepository.UpdateAsync(existingComment.Id, existingComment);
            await _unitOfWork.SaveChangesAsync();

            if (updatedComment is null)
                throw new ApplicationException("An error occured. Unable to update comment");

            return new CommentResponseDto
            {
                Id = updatedComment.Id,
                UserId = user.Id,
                Content = updatedComment.Content,
                Username = updatedComment.User.Username,
                CommentedAt = updatedComment.CommentedAt,
                LastUpdatedAt = updatedComment.LastUpdatedAt,
                IsDeleted = updatedComment.IsDeleted,
            };

        }

        public async Task<OperationResultDto> InitialDeleteCommentAsync(Guid commentId, Guid userId, bool isAdmin)
        {
            var existingComment = await _commentRepository.GetByIdAsync(commentId);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException($"User with the id of {userId} not found");

            if (existingComment is null)
                throw new NotFoundException($"Comment with the id of {commentId} not found");

            if (existingComment.UserId != userId && !isAdmin)
                throw new UnauthorizedException("You are unauthorize to delete this comment");

            existingComment.IsDeleted = true;

            var deletedComment = await _commentRepository.UpdateAsync(existingComment.Id, existingComment);
            await _unitOfWork.SaveChangesAsync();

            if (deletedComment is null) 
                throw new ApplicationException("An error occured. Unable to delete the comment");

            return OperationResult.Success("Successfuly deleted comment");

        }

        public async Task<OperationResultDto> DeletePostAsync(Guid commentId, bool isAdmin)
        {
            var existingComment = await _commentRepository.GetByIdAsync(commentId);

            if (existingComment is null)
                throw new NotFoundException($"Comment not found with the id {commentId}");

            if (!isAdmin)
                throw new UnauthorizedException("You are unauthorized to completely delete this comment");

            await _commentRepository.DeleteAsync(existingComment);

            return OperationResult.Success("Successfully deleted the comment");
        }

        private static void UpdateCommentField(Comment existingComment, UpdateCommentDto comment)
        {
            if (!string.IsNullOrWhiteSpace(comment.Content)) existingComment.Content = comment.Content;

            existingComment.LastUpdatedAt = DateTime.UtcNow;
        }
    }
}
