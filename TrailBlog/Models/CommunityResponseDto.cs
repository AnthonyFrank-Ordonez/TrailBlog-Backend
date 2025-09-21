namespace TrailBlog.Api.Models
{
    public class CommunityResponseDto
    {
        public Guid Id { get; set; }
        public string CommunityName { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public List<PostResponseDto> Posts { get; set; } = new List<PostResponseDto>();
    }
}
        