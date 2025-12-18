namespace TrailBlog.Api.Models
{
    public sealed class UnifiedSearchResultDto
    {
        public IEnumerable<PostSearchResultDto> Posts { get; set; } = new List<PostSearchResultDto>();
        public IEnumerable<CommunitySearchResultDto> Communities { get; set; } = new List<CommunitySearchResultDto>();
    }
}