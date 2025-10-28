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
    public class CommentController(ICommentService commentService) : ControllerBase
    {
        private readonly ICommentService _commentService = commentService;


        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetDeletedComments()
        {
            var deletedComments = await _commentService.GetDeletedComments();

            return Ok(deletedComments);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<CommentResponseDto>> AddComment(CommentDto comment)
        {
            var userId = GetCurrentUserId();
            var newComment = await _commentService.AddCommentAsync(userId, comment);

            return Ok(newComment);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<CommentResponseDto>> EditComment(Guid id, UpdateCommentDto comment)
        {
            var userId = GetCurrentUserId();
            var updateComment = await _commentService.EditCommentAsync(id, userId, comment);

            return Ok(updateComment);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin,User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> InitialDeleteComment(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _commentService.InitialDeleteCommentAsync(id, userId, isAdmin);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> DeletePost(Guid id)
        {
            var result = await _commentService.DeletePostAsync(id);
            return Ok(result);
        }


        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }
}
