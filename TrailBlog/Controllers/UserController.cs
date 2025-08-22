using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrailBlog.Entities;
using TrailBlog.Models;
using TrailBlog.Services;

namespace TrailBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto?>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();

            if (users is null || !users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto>> GetUser(Guid id)
        {
            var user = await _userService.GetUserAsync(id);

            if (user is null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [HttpPost("{id}/revoke")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RevokeUser(Guid id)
        {
            var user = await _userService.RevokedUserAsync(id);
            if (user is null)
            {
                return NotFound("User not found.");
            }

            return Ok("User revoked successfully.");
        }


    }
}
