using Microsoft.AspNetCore.Mvc;
using TrailBlog.Api.Models;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Services
{
    public interface IPostService
    {
        Task<IEnumerable<PostResponseDto?>> GetPostsAsync();
        Task<PostResponseDto?> GetPostAsync(Guid id);
        Task<PostResponseDto?> CreatePostAsync(PostDto post, Guid userId);
        Task<OperationResultDto> UpdatePostAsync(Guid id, Guid userId, PostDto post, bool isAdmin = false);
        Task<OperationResultDto> DeletePostAsync(Guid id, Guid userId, bool isAdmin = false);
        Task<IEnumerable<PostResponseDto>> GetRecentPostsAsync(int page, int pageSize);

    }
}
