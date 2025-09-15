namespace TrailBlog.Models
{
    public class PostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CommunityName { get; set; } = string.Empty;
        public Guid CommunityId { get; set; } = Guid.Empty;
    }
}
