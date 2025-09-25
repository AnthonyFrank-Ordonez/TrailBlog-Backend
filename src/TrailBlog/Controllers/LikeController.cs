using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using TrailBlog.Api.Models;
using TrailBlog.Api.Services;

namespace TrailBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController(ILikeService likeService) : ControllerBase
    {
        private readonly ILikeService _likeService = likeService;

        [HttpPost("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> AddLike(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _likeService.AddPostLikeAsync(userId, id);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> RemoveLike(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _likeService.RemovePostLikeAsync(userId, id);

            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }
}
