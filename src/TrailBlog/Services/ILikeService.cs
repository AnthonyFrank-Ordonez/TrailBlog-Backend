using TrailBlog.Api.Entities;
using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface ILikeService
    {
        Task<PostResponseDto> AddPostLikeAsync(Guid userId, Guid postId);
        Task<PostResponseDto> AddPostDislikeAsync(Guid userId, Guid postId);
    }
}
