namespace TrailBlog.Models
{
    public class CommunityBlogsDto
    {
        public Guid CommunityId { get; set; }
        public string CommunityName { get; set; } = string.Empty;
        public List<PostDto> Posts { get; set; } = new List<PostDto>();
    }
}
    