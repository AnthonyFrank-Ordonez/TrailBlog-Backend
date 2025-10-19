using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrailBlog.Api.Models;
using TrailBlog.Api.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace TrailBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController(IPostService postService) : ControllerBase
    {

        private readonly IPostService _postService = postService;

        [HttpGet]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetPostsPaged([FromQuery] int page, [FromQuery] int pageSize)
        {
            var userId = GetCurrentUserId();
            var posts = await _postService.GetPostsPagedAsync(userId, page, pageSize);

            return Ok(posts);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PostResponseDto?>> GetPost(Guid id)
        {
            var userId = GetCurrentUserId();
            var post = await _postService.GetPostAsync(id, userId);
            return Ok(post);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PostResponseDto>> CreatePost(PostDto post)
        {
            var userId = GetCurrentUserId();
            var createdPost = await _postService.CreatePostAsync(post, userId);

            return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> UpdatePost(Guid id, UpdatePostDto post)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.UpdatePostAsync(id, userId, post, isAdmin);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> DeletePost(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.DeletePostAsync(id, userId, isAdmin);

            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }
}
