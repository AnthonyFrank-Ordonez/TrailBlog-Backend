using TrailBlog.Api.Exceptions;
using TrailBlog.Api.Models;

namespace TrailBlog.Api.Services
{
    public class SearchService(
        IPostService postService,
        ICommunityService communityService,
        ILogger<SearchService> logger): ISearchService
    {
        private readonly IPostService _postService = postService;
        private readonly ICommunityService _communityService = communityService;
        private readonly ILogger<SearchService> _logger = logger;

        public async Task<UnifiedSearchResultDto> UnifiedSearchAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
                throw new ApiException("Search query cannot be null or empty.");

            var postTask = await _postService.SearchPostsAsync(query);
            var communityTask = await _communityService.SearchCommunitysAsync(query);

            return new UnifiedSearchResultDto
            {
                Posts = postTask,
                Communities = communityTask,
            };


        }
    }
}
