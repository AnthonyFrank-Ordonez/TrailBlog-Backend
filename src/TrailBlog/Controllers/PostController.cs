using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrailBlog.Api.Models;
using TrailBlog.Api.Services;
using Microsoft.AspNetCore.RateLimiting;
using TrailBlog.Api.Extensions;

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
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetPostsPaged([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? sessionId = null)
        {
            var userId = this.GetCurrentUserId();
            var posts = await _postService.GetPostsPagedAsync(userId, page, pageSize, sessionId);

            return Ok(posts);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PostResponseDto?>> GetPost(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var post = await _postService.GetPostAsync(id, userId);
            return Ok(post);
        }

        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PostResponseDto>> GetPostBySlug(string slug)
        {
            var userId = this.GetCurrentUserId();
            var post = await _postService.GetPostBySlugAsync(slug, userId);

            return Ok(post);

        }

        [HttpGet("recent-viewed-posts")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<IEnumerable<RecentViewedPostDto>>> GetRecentlyViewedPosts([FromQuery] int? count)
        {
            var userId = this.GetRequiredUserId();
            var recentPosts = await _postService.GetRecentlyViewedPostAsync(userId, count ?? 10);

            return Ok(recentPosts);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PostResponseDto>> CreatePost(PostDto post)
        {
            var userId = this.GetRequiredUserId();
            var createdPost = await _postService.CreatePostAsync(post, userId);

            return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
        }

        [HttpPost("{id}/reaction")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PostResponseDto>> AddReaction(Guid id, AddReactionDto reaction)
        {
            var userId = this.GetRequiredUserId();
            var result = await _postService.TogglePostReactionAsync(userId, id, reaction);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> UpdatePost(Guid id, UpdatePostDto post)
        {
            var userId = this.GetRequiredUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.UpdatePostAsync(id, userId, post, isAdmin);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> DeletePost(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.DeletePostAsync(id, userId, isAdmin);

            return Ok(result);
        }
    }
}
