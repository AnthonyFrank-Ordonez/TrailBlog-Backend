using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrailBlog.Api.Models;
using TrailBlog.Api.Services;
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
        public async Task<ActionResult<PagedResultDto<PublicUserResponseDto>>> GetUsersPaged([FromQuery] int page, [FromQuery] int pageSize)
        {
            var users = await _userService.GetUsersPagedAsync(page, pageSize);

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PublicUserResponseDto?>> GetUser(Guid id)
        {
            var user = await _userService.GetUserAsync(id);

            return user;
        }


        [HttpGet("roles")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<PagedResultDto<UserResponseDto>?>> GetAllUsersWithRoles([FromQuery] int page, [FromQuery] int pageSize)
        {
            var users = await _userService.GetUsersWithRolesPagedAsync(page, pageSize);

            return Ok(users);
        }

        [HttpGet("{id}/role")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<UserResponseDto>> GetUserWithRoles(Guid id)
        {
            var user = await _userService.GetUserWithRolesAsync(id);

            return Ok(user);
        }

        [HttpGet("admins")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllAdminUsers()
        {
            var adminUsers = await _userService.GetAllAdminUsersAsync();

            return Ok(adminUsers);
        }

        [HttpPost("{id}/revoke")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> RevokeUser(Guid id)
        {
            var result = await _userService.RevokedUserAsync(id);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);

            return Ok(result);
        }


    }
}
