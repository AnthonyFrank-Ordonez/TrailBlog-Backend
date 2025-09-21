using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrailBlog.Api.Models;
using TrailBlog.Api.Services;
using TrailBlog.Api.Entities;
using TrailBlog.Api.Helpers;

namespace TrailBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController(IPostService postService) : ControllerBase
    {

        private readonly IPostService _postService = postService;

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PostResponseDto?>>> GetPosts()
        {
            var posts = await _postService.GetPostsAsync();

            if (posts is null || !posts.Any())
            {
                return NotFound(OperationResult.Failure("No posts found"));
            }

            return Ok(posts);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostResponseDto?>> GetPost(Guid id)
        {
            var post = await _postService.GetPostAsync(id);

            if (post is null)
            {
                return NotFound(OperationResult.Failure("Post not found"));
            }

            return Ok(post);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<PostResponseDto?>> CreatePost(PostDto post)
        {
            var userId = GetCurrentUserId();
            var createdPost = await _postService.CreatePostAsync(post, userId);

            if (createdPost is null)
            {
                return BadRequest(OperationResult.Failure("Invalid post data"));
            }

            return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<OperationResultDto>> UpdatePost(Guid id, PostDto post)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.UpdatePostAsync(id, userId, post, isAdmin);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<OperationResultDto>> DeletePost(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var result = await _postService.DeletePostAsync(id, userId, isAdmin);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("recent")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetRecentPosts([FromQuery] int page, [FromQuery] int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                page = 1;
                pageSize = 10;
            }

            var recentPosts = await _postService.GetRecentPostsAsync(page, pageSize);

            if (recentPosts is null || !recentPosts.Any())
            {
                return NotFound(OperationResult.Failure("No recent posts found"));
            }

            return Ok(recentPosts);
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }
}
