namespace TrailBlog.Api.Models
{
    public sealed class CommentResponseDto
    {
        public Guid Id { get; set; }
        public String Content { get; set; } = string.Empty;
        public DateTime CommentedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }   
}
