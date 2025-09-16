using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrailBlog.Entities;
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
        public async Task<ActionResult<IEnumerable<Community>>> GetAllCommunities()
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
        public async Task<ActionResult<Community?>> GetCommunity(Guid id)
        {
            var community = await _communityService.GetCommunity(id);

            if (community is null)
            {
                return NotFound("Community Not Found!");
            }

            return Ok(community);
        }
    }
}
