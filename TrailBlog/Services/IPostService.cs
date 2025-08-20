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
        Task<Post?> UpdatePostAsync(Guid id, PostDto Post, Guid userId);
        Task<Post?> DeletePostAsync(Guid id, Guid userId, bool IsAdmin = false);
    }
}
