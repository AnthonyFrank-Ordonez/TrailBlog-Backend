using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrailBlog.Entities;
using TrailBlog.Models;
using TrailBlog.Services;

namespace TrailBlog.Controllers
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
        public async Task<ActionResult<IEnumerable<CommunityResponseDto>>> GetAllCommunities()
        {
            var communities = await _communityService.GetAllCommunitiesAsync();

            if (communities is null || !communities.Any())
            {
                return NotFound("No Communities Found!");
            }

            return Ok(communities);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CommunityResponseDto?>> GetCommunity(Guid id)
        {
            var community = await _communityService.GetCommunityAsync(id);

            if (community is null)
            {
                return NotFound("Community Not Found!");
            }

            return Ok(community);
        }

        [HttpGet("user")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<CommunityResponseDto>>> GetUserCommunities()
        {
            var userId = GetCurrentUserId();

            var userCommunities = await _communityService.GetUserCommunitiesAsync(userId);

            if (userCommunities is null || !userCommunities.Any())
            {
                return NotFound("No Communities Found for the User!");
            }

            return Ok(userCommunities);
        }

        [HttpGet("{id}/members")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetCommunityMembers(Guid id)
        {
            var members = await _communityService.GetCommunityMembersAsync(id);

            if (members is null || !members.Any())
            {
                return NotFound("No community members found!");
            }

            return Ok(members);
        }

        [HttpPost("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CommunityResponseDto>>> SearchCommunities([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty!");
            }

            var communities = await _communityService.SearchCommunitiesAsync(query);

            if (communities is null || !communities.Any())
            {
                return NotFound("No communities found matching the query");
            }

            return Ok(communities);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<CommunityResponseDto?>> CreateCommunity(CommunityDto community)
        {
            var userId = GetCurrentUserId();

            var createdCommunity = await _communityService.CreateCommunityAsync(community, userId);

            if (createdCommunity is null)
            {
                return BadRequest("Failed to create community");
            }

            return CreatedAtAction(nameof(GetCommunity), new { id = createdCommunity.Id }, createdCommunity);
        }

        [HttpPut("id")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<OperationResultDto>> UpdateCommunity(Guid id, CommunityDto community)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var result = await _communityService.UpdateCommunityAsync(id, userId, community, isAdmin);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<OperationResultDto>> DeleteCommunity(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var result = await _communityService.DeleteCommunityAsync(id, userId, isAdmin);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }



        [HttpPost("{id}/join")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<OperationResultDto>> JoinCommunity(Guid id)
        {
            var userId = GetCurrentUserId();

            var result = await _communityService.JoinCommunityAsync(id, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("{id}/leave")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<OperationResultDto>> LeaveCommunity(Guid id)
        {
            var userId = GetCurrentUserId();

            var result = await _communityService.LeaveCommunityAsync(id, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }

    }
}
