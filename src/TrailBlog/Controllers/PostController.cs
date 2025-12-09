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

        [HttpGet("popular")]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetPopularPosts([FromQuery] int page, [FromQuery] int pageSize)
        {
            var userId = this.GetCurrentUserId();
            var posts = await _postService.GetPopularPostsPagedAsync(userId, page, pageSize);
            return Ok(posts);
        }

        [HttpGet("explore")]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetExplorePosts([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? sessionId = null)
        {
            var userId = this.GetCurrentUserId();
            var posts = await _postService.GetExploredPostsPagedAsync(userId, page, pageSize, sessionId);

            return Ok(posts);
        }

        [HttpGet("drafts")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetDraftPostsPaged([FromQuery] int page, [FromQuery] int pageSize)
        {
            var userId = this.GetRequiredUserId();
            var posts = await _postService.GetUserDraftsPagedAsync(userId, page, pageSize);
            return Ok(posts);
        }

        [HttpGet("archived")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetArchivedPostsPaged([FromQuery] int page, [FromQuery] int pageSize)
        {
            var userId = this.GetRequiredUserId();
            var posts = await _postService.GetUserArchivePostsAsync(userId, page, pageSize);

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

        [HttpGet("saved")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetSavedPosts([FromQuery] int page, [FromQuery] int pageSize)
        {
            var userId = this.GetRequiredUserId();
            var savedPosts = await _postService.GetSavedPostsPagedAsync(userId, page, pageSize);

            return Ok(savedPosts);
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

        [HttpPost("{id}/saved")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PostResponseDto>> SavedPost(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var result = await _postService.SavedPostAsync(userId, id);

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

        [HttpPatch("drafts/{id}/publish")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> PublishDraftPost(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var result = await _postService.UpdateDraftAsync(id, userId);

            return Ok(result);
        }

        [HttpPatch("{id}/archive")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> ArchivePost(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var result = await _postService.ArchivePostAsync(id, userId);

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

        [HttpDelete("{id}/saved")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> DeleteSavedPosts(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var result = await _postService.DeleteSavedPostAsync(userId, id);

            return Ok(result);
        }

        [HttpDelete("recent-viewed-posts")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> DeleteAllRecentViewPosts()
        {
            var userId = this.GetRequiredUserId();
            var result = await _postService.DeleteAllRecentViewedPostAsync(userId);

            return Ok(result);
        }

        [HttpDelete("drafts/{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> DeleteDraftPost(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.DeleteDraftAsync(id, userId, isAdmin);

            return Ok(result);
        }
    }
}
