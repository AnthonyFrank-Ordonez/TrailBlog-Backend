namespace TrailBlog.Models
{
    public class CommunityBlogsResponseDto
    {
        public Guid Id { get; set; }
        public string CommunityName { get; set; } = string.Empty;
        public List<PostResponseDto> Posts { get; set; } = new List<PostResponseDto>();
    }
}
        