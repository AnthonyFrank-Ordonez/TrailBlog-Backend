using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface IPostService
    {
        Task<PagedResultDto<PostResponseDto>> GetPostsPagedAsync(Guid? userId, int page = 1, int pageSize = 10, string? sessionId = null);
        Task<PagedResultDto<PostResponseDto>> GetPopularPostsPagedAsync(Guid? userId, int page = 1, int pageSize = 10);
        Task<PagedResultDto<PostResponseDto>> GetSavedPostsPagedAsync(Guid userId, int page = 1, int pageSize = 10);
        Task<PostResponseDto?> GetPostAsync(Guid id, Guid userId);
        Task<PostResponseDto?> GetPostBySlugAsync(string slug, Guid? userId);
        Task<IEnumerable<RecentViewedPostDto>> GetRecentlyViewedPostAsync(Guid userId, int count = 10);
        Task<PostResponseDto> CreatePostAsync(PostDto post, Guid userId);
        Task<PostResponseDto> SavedPostAsync(Guid userId, Guid postId);
        Task<PostResponseDto> TogglePostReactionAsync(Guid userId, Guid postId, AddReactionDto reaction);
        Task<OperationResultDto> UpdatePostAsync(Guid id, Guid userId, UpdatePostDto post, bool isAdmin = false);
        Task<OperationResultDto> DeletePostAsync(Guid id, Guid userId, bool isAdmin = false);
        Task<OperationResultDto> DeleteSavedPostAsync(Guid userId, Guid postId);
        Task<OperationResultDto> DeleteAllRecentViewedPostAsync(Guid userid);

    }
}
