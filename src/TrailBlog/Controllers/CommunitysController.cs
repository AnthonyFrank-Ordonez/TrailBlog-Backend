using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrailBlog.Api.Models;
using TrailBlog.Api.Services;
using Microsoft.AspNetCore.RateLimiting;
using TrailBlog.Api.Extensions;

namespace TrailBlog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunitysController : ControllerBase
    {

        private readonly ICommunityService _communityService;

        public CommunitysController(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        [HttpGet]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<IEnumerable<CommunityResponseDto>>> GetAllCommunities([FromQuery] int page, [FromQuery] int pageSize)
        {
            var communities = await _communityService.GetCommunitiesPagedAsync(page, pageSize);

            return Ok(communities);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<CommunityResponseDto?>> GetCommunity(Guid id, [FromQuery] int page, [FromQuery] int pageSize)
        {
            var userId = this.GetCurrentUserId();
            var community = await _communityService.GetCommunityPostsPagedAsync(id, userId, page, pageSize);

            return Ok(community);
        }

        [HttpGet("user")]
        [Authorize(Roles = "Admin, User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<IEnumerable<CommunityResponseDto>>> GetUserCommunities()
        {
            var userId = this.GetRequiredUserId();
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
            var userId = this.GetRequiredUserId();
            var createdCommunity = await _communityService.CreateCommunityAsync(community, userId);

            return CreatedAtAction(nameof(GetCommunity), new { id = createdCommunity.Id }, createdCommunity);
        }

        [HttpPost("{id}/favorite")]
        [Authorize(Roles = "Admin, User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<CommunityResponseDto>> FavoriteCommunity(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var userCommunity = await _communityService.FavoriteCommunityAsync(id, userId);

            return Ok(userCommunity);
        }

        [HttpPost("{id}/unfavorite")]
        [Authorize(Roles = "Admin, User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<CommunityResponseDto>> UnfavoriteCommunity(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var userCommunity = await _communityService.UnfavoriteCommunityAsync(id, userId);

            return Ok(userCommunity);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> UpdateCommunity(Guid id, CommunityDto community)
        {
            var userId = this.GetRequiredUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _communityService.UpdateCommunityAsync(id, userId, community, isAdmin);


            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, User")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> DeleteCommunity(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _communityService.DeleteCommunityAsync(id, userId, isAdmin);

 
            return Ok(result);
        }



        [HttpPost("{id}/join")]
        [Authorize(Roles = "User, Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<CommunityResponseDto>> JoinCommunity(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var community = await _communityService.JoinCommunityAsync(id, userId);


            return Ok(community);
        }

        [HttpPost("{id}/leave")]
        [Authorize(Roles = "User, Admin")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<OperationResultDto>> LeaveCommunity(Guid id)
        {
            var userId = this.GetRequiredUserId();
            var result = await _communityService.LeaveCommunityAsync(id, userId);

            return Ok(result);
        }

    }
}
