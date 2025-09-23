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
            return Ok(posts);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostResponseDto?>> GetPost(Guid id)
        {
            var post = await _postService.GetPostAsync(id);
            return Ok(post);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<PostResponseDto>> CreatePost(PostDto post)
        {
            var userId = GetCurrentUserId();
            var createdPost = await _postService.CreatePostAsync(post, userId);

            return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<OperationResultDto>> UpdatePost(Guid id, PostDto post)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.UpdatePostAsync(id, userId, post, isAdmin);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<OperationResultDto>> DeletePost(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _postService.DeletePostAsync(id, userId, isAdmin);

            return Ok(result);
        }

        [HttpGet("recent")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetRecentPosts([FromQuery] int page, [FromQuery] int pageSize)
        {
            var recentPosts = await _postService.GetRecentPostsAsync(page, pageSize);

            return Ok(recentPosts);
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }
}
