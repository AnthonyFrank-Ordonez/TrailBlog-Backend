using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface ICommentService
    {
        Task<PagedResultDto<CommentResponseDto>> GetUserCommentsAsync(Guid userId, int page, int pageSize);
        Task<IEnumerable<CommentResponseDto>> GetDeletedCommentsAsync();
        Task<CommentResponseDto> AddCommentAsync(Guid userId, CommentDto comment);
        Task<CommentResponseDto> EditCommentAsync(Guid commentId, Guid userId, UpdateCommentDto comment);
        Task<OperationResultDto> InitialDeleteCommentAsync(Guid commentId, Guid userId, bool isAdmin = false);
        Task<OperationResultDto> DeletePostAsync(Guid commentId, bool isAdmin = false);
    }
}
