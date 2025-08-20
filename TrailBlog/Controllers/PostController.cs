using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrailBlog.Entities;
using TrailBlog.Models;
using TrailBlog.Services;

namespace TrailBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {

        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PostResponseDto?>>> GetPosts()
        {
            var posts = await _postService.GetPostsAsync();

            if (posts is null || !posts.Any())
            {
                return NotFound("No posts found.");
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
                return NotFound("Post not found");
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
                return BadRequest("Invalid post data.");
            }

            return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdatePost(Guid id, PostDto post)
        {
            var userId = GetCurrentUserId();
            var updatedPost = await _postService.UpdatePostAsync(id, post, userId);

            if (updatedPost is null)
            {
                return NotFound("Post not Found");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var result = await _postService.DeletePostAsync(id, userId, isAdmin);
            if (result is null)
            {
                return NotFound("Post not found");
            }
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }
}
