using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrailBlog.Api.Models;
using TrailBlog.Api.Services;
using TrailBlog.Api.Entities;
using TrailBlog.Api.Helpers;
using Microsoft.AspNetCore.RateLimiting;

namespace TrailBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<UserResponseDto>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<UserResponseDto?>> GetUser(Guid id)
        {
            var user = await _userService.GetUserAsync(id);

            return user;
        }


        [HttpGet("roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto?>> GetAllUsersWithRoles()
        {
            var users = await _userService.GetAllUsersWithRolesAsync();

            return Ok(users);
        }

        [HttpGet("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto>> GetUserWithRoles(Guid id)
        {
            var user = await _userService.GetUserWithRolesAsync(id);

            return Ok(user);
        }

        [HttpGet("admins")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllAdminUsers()
        {
            var adminUsers = await _userService.GetAllAdminUsersAsync();

            return Ok(adminUsers);
        }

        [HttpPost("{id}/revoke")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OperationResultDto>> RevokeUser(Guid id)
        {
            var result = await _userService.RevokedUserAsync(id);

            return Ok(result);
        }


    }
}
