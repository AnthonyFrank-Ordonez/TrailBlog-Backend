using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface IPostService
    {
        Task<PagedResultDto<PostResponseDto>> GetPostsPagedAsync(int page = 1, int pageSize = 10);
        Task<PostResponseDto?> GetPostAsync(Guid id);
        Task<PostResponseDto> CreatePostAsync(PostDto post, Guid userId);
        Task<OperationResultDto> UpdatePostAsync(Guid id, Guid userId, UpdatePostDto post, bool isAdmin = false);
        Task<OperationResultDto> DeletePostAsync(Guid id, Guid userId, bool isAdmin = false);
    }
}
