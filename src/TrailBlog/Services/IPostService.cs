using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface IPostService
    {
        Task<PagedResultDto<PostResponseDto>> GetPostsPagedAsync(Guid userId, int page = 1, int pageSize = 10);
        Task<PostResponseDto?> GetPostAsync(Guid id, Guid userId);
        Task<PostResponseDto> CreatePostAsync(PostDto post, Guid userId);
        Task<PostResponseDto> AddPostLikeAsync(Guid userId, Guid postId);
        Task<PostResponseDto> AddPostDisLikeAsync(Guid userId, Guid postId);
        Task<OperationResultDto> UpdatePostAsync(Guid id, Guid userId, UpdatePostDto post, bool isAdmin = false);
        Task<OperationResultDto> DeletePostAsync(Guid id, Guid userId, bool isAdmin = false);

    }
}
