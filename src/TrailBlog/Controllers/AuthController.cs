using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        [EnableRateLimiting("auth-operations")]
        public async Task<ActionResult<AuthResultDto>> Register(RegisterDto request)
        {
            var result = await _authService.RegisterAsync(request);

            return Ok(result);
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth-operations")]
        public async Task<ActionResult<AuthResultDto>> Login(LoginDto request)
        {
            var result = await _authService.LoginAsync(request);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<AuthResultDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        public async Task<IActionResult> LogoutUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var result = await _authService.LogoutAsync(Guid.Parse(userId));

            return Ok(result);
        }


        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> AssignRole(AssignRoleDto request)
        {
            var result = await _authService.AssignRoleAsync(request);

            return Ok(result);
        }

    }
}
