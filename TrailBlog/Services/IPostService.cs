using Microsoft.AspNetCore.Mvc;
using TrailBlog.Entities;
using TrailBlog.Models;

namespace TrailBlog.Services
{
    public interface IPostService
    {
        Task<IEnumerable<PostResponseDto?>> GetPostsAsync();
        Task<PostResponseDto?> GetPostAsync(Guid id);
        Task<PostResponseDto?> CreatePostAsync(PostDto post, Guid userId);
        Task<OperationResultDto> UpdatePostAsync(Guid id, Guid userId, PostDto post, bool isAdmin = false);
        Task<OperationResultDto> DeletePostAsync(Guid id, Guid userId, bool isAdmin = false);
        Task<List<CommunityResponseDto>> GetAllCommunityBlogsAsync();
        Task<List<PostResponseDto>> GetRecentPostsAsync(int page, int pageSize);

    }
}
