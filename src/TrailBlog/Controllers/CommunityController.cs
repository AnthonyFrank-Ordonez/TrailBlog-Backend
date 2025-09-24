using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrailBlog.Api.Models;
using TrailBlog.Api.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace TrailBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {

        private readonly ICommunityService _communityService;

        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        [HttpGet]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<IEnumerable<CommunityResponseDto>>> GetAllCommunities()
        {
            var communities = await _communityService.GetAllCommunitiesAsync();

            return Ok(communities);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<CommunityResponseDto?>> GetCommunity(Guid id)
        {
            var community = await _communityService.GetCommunityAsync(id);

            return Ok(community);
        }

        [HttpGet("user")]
        [Authorize(Roles = "Admin, User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<IEnumerable<CommunityResponseDto>>> GetUserCommunities()
        {
            var userId = GetCurrentUserId();
            var userCommunities = await _communityService.GetUserCommunitiesAsync(userId);

            return Ok(userCommunities);
        }

        [HttpGet("{id}/members")]
        [Authorize(Roles = "Admin, User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetCommunityMembers(Guid id)
        {
            var members = await _communityService.GetCommunityMembersAsync(id);

            return Ok(members);
        }

        [HttpGet("communities")]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<IEnumerable<CommunityResponseDto>>> GetAllCommunityPosts()
        {
            var communityPosts = await _communityService.GetAllCommunityPostsAsync();

            return Ok(communityPosts);
        }

        [HttpPost("search")]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<IEnumerable<CommunityResponseDto>>> SearchCommunities([FromQuery] string query)
        {
            var communities = await _communityService.SearchCommunitiesAsync(query);

            return Ok(communities);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<CommunityResponseDto>> CreateCommunity(CommunityDto community)
        {
            var userId = GetCurrentUserId();
            var createdCommunity = await _communityService.CreateCommunityAsync(community, userId);

            return CreatedAtAction(nameof(GetCommunity), new { id = createdCommunity.Id }, createdCommunity);
        }

        [HttpPut("id")]
        [Authorize(Roles = "Admin, User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> UpdateCommunity(Guid id, CommunityDto community)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _communityService.UpdateCommunityAsync(id, userId, community, isAdmin);


            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> DeleteCommunity(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _communityService.DeleteCommunityAsync(id, userId, isAdmin);

 
            return Ok(result);
        }



        [HttpPost("{id}/join")]
        [Authorize(Roles = "User, Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> JoinCommunity(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _communityService.JoinCommunityAsync(id, userId);


            return Ok(result);
        }

        [HttpPost("{id}/leave")]
        [Authorize(Roles = "User, Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> LeaveCommunity(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _communityService.LeaveCommunityAsync(id, userId);

            return Ok(result);
        }


        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }

    }
}
