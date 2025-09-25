using TrailBlog.Api.Entities;
using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public interface ILikeService
    {
        Task<OperationResultDto> AddPostLikeAsync(Guid userId, Guid postId);
        Task<OperationResultDto> RemovePostLikeAsync(Guid userId, Guid postId);
    }
}
