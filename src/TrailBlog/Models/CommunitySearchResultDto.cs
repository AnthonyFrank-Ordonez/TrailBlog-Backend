namespace TrailBlog.Api.Models
{
    public sealed class CommunitySearchResultDto
    {
        public Guid Id { get; set; }
        public string CommunityName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
