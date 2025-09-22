using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrailBlog.Api.Models;
using TrailBlog.Api.Services;

namespace TrailBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResultDto>> Register(RegisterDto request)
        {
            var result = await _authService.RegisterAsync(request);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> Login(LoginDto request)
        {
            var result = await _authService.LoginAsync(request);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResultDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            await _authService.LogoutAsync(Guid.Parse(userId));

            return Ok("Logout Successfully!");
        }


        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(AssignRoleDto request)
        {
            var success = await _authService.AssignRoleAsync(request);

            return Ok("Role assigned succesfully");
        }

    }
}
